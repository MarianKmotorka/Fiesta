using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Exceptions;
using Fiesta.Application.Common.Models;
using Fiesta.Application.Features.Auth;
using Fiesta.Application.Features.Auth.CommonDtos;
using Fiesta.Infrastracture.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fiesta.Infrastracture.Auth
{
    public partial class AuthService
    {
        public async Task<Result<(string accessToken, string refreshToken)>> Login(string emailOrUsername, string password, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(emailOrUsername.Trim());

            if (user is null)
            {
                user = await _userManager.Users.SingleOrDefaultAsync(x => x.UserName == emailOrUsername.Trim());

                if (user is null)
                    return Result<(string, string)>.Failure(ErrorCodes.InvalidLoginCredentials);
            }

            if (!user.AuthProvider.HasFlag(AuthProviderEnum.EmailAndPassword))
                return Result<(string, string)>.Failure(ErrorCodes.InvalidAuthProvider);

            var passValid = await _userManager.CheckPasswordAsync(user, password);
            if (!passValid)
                return Result<(string, string)>.Failure(ErrorCodes.InvalidLoginCredentials);

            if (!user.EmailConfirmed)
                throw new BadRequestException(ErrorCodes.EmailIsNotVerified);

            return Result.Success(await Login(user, cancellationToken));
        }

        public async Task<Result> Logout(string refreshToken, CancellationToken cancellationToken)
        {
            var userId = GetPrincipalFromJwt(refreshToken)?.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

            if (userId is null)
                return Result.Failure(ErrorCodes.InvalidRefreshToken);

            var user = await _db.Users.SingleAsync(x => x.Id == userId, cancellationToken);

            if (refreshToken != user.RefreshToken)
                return Result.Failure(ErrorCodes.InvalidRefreshToken);

            user.RefreshToken = null;
            await _db.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result<(string accessToken, string refreshToken, bool authUserCreated, string userId, string username)>> LoginOrRegister(GoogleUserInfoModel model, CancellationToken cancellationToken)
        {
            var user = await _db.Users.WhereSomeEmailIs(model.Email).SingleOrDefaultAsync(cancellationToken);

            if (user is not null)
            {
                if (!user.AuthProvider.HasFlag(AuthProviderEnum.Google))
                    return Result<(string, string, bool, string, string)>.Failure(ErrorCodes.InvalidAuthProvider);
                else if (user.GoogleEmail != model.Email)
                    return Result<(string, string, bool, string, string)>.Failure(ErrorCodes.AccountAlreadyConnectedToGoogleWithDifferentEmail);

                var (accessToken, refreshToken) = await Login(user, cancellationToken);
                return Result.Success((accessToken, refreshToken, false, user.Id, user.UserName));
            }

            var newUser = new AuthUser(model.Email, AuthProviderEnum.Google, await GenerateUsernamne(model.Email, cancellationToken)) { EmailConfirmed = model.IsEmailVerified };
            var result = await _userManager.CreateAsync(newUser);

            if (!result.Succeeded)
                return Result<(string, string, bool, string, string)>.Failure(result.Errors.Select(x => x.Code));

            var loginResult = await Login(newUser, cancellationToken);
            return Result.Success((loginResult.accessToken, loginResult.refreshToken, true, newUser.Id, newUser.UserName));
        }

        public async Task<Result<(string accessToken, string refreshToken)>> RefreshJwt(string refreshToken, CancellationToken cancellationToken)
        {
            var validatedRefreshToken = GetPrincipalFromJwt(refreshToken);

            if (validatedRefreshToken?.Claims.SingleOrDefault(x => x.Type == FiestaClaims.IsRefreshToken) is null)
                return Result<(string, string)>.Failure(ErrorCodes.InvalidRefreshToken);

            var expiryDateUnix =
                    long.Parse(validatedRefreshToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateUtc =
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expiryDateUnix);

            if (expiryDateUtc < DateTime.UtcNow)
                return Result<(string, string)>.Failure(ErrorCodes.RefreshTokenExpired);

            var appUserId = validatedRefreshToken.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var appUser = await _db.Users.SingleAsync(x => x.Id == appUserId, cancellationToken);
            var storedRefreshToken = appUser.RefreshToken;

            if (storedRefreshToken != refreshToken)
                return Result<(string, string)>.Failure(ErrorCodes.InvalidRefreshToken);

            return Result.Success(await Login(appUser, cancellationToken));
        }

        public async Task<Result<(string userId, string username)>> Register(RegisterWithEmailAndPassword.Command command, CancellationToken cancellationToken)
        {
            var newUser = new AuthUser(command.Email, AuthProviderEnum.EmailAndPassword, await GenerateUsernamne(command.Email, cancellationToken));

            var result = await _userManager.CreateAsync(newUser, command.Password);
            if (!result.Succeeded)
                return Result<(string, string)>.Failure(result.Errors.Select(x => x.Description));

            await _db.SaveChangesAsync(cancellationToken);
            return Result.Success((newUser.Id, newUser.UserName));
        }

        private async Task<(string accessToken, string refreshToken)> Login(AuthUser user, CancellationToken cancellationToken)
        {
            var accessToken = CreateAccessToken(user);
            var refreshToken = CreateRefreshToken(user);

            user.RefreshToken = refreshToken;
            await _db.SaveChangesAsync(cancellationToken);

            return (accessToken, refreshToken);
        }

        private string CreateRefreshToken(AuthUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtOptions.Secret);
            var jti = Guid.NewGuid().ToString();
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti,  jti),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Iss, _jwtOptions.Issuer),
                new Claim(FiestaClaims.IsRefreshToken, "true")
            };

            var refreshTokenObject = new JwtSecurityToken(
                _jwtOptions.Issuer,
                null,
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.Add(_jwtOptions.RefreshTokenLifeTime),
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
                );

            return tokenHandler.WriteToken(refreshTokenObject);
        }

        private string CreateAccessToken(AuthUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtOptions.Secret);
            var jti = Guid.NewGuid().ToString();
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti,  jti),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Iss, _jwtOptions.Issuer),
                new Claim(FiestaClaims.FiestaRole, user.Role.ToString()),
                new Claim(FiestaClaims.IsAccessToken,"true")
            };

            var accessTokenObject = new JwtSecurityToken(
                _jwtOptions.Issuer,
                null,
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.Add(_jwtOptions.TokenLifeTime),
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
                );

            return tokenHandler.WriteToken(accessTokenObject);
        }

        private ClaimsPrincipal GetPrincipalFromJwt(string jwt)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = _tokenValidationParameters.Clone();
            tokenValidationParameters.ValidateLifetime = false;

            try
            {
                var principal = jwtHandler.ValidateToken(jwt, tokenValidationParameters, out var validatedJwt);

                var hasJwtValidSecurityAlgorithm =
                    (validatedJwt is JwtSecurityToken jwtSecurityToken) && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                if (!hasJwtValidSecurityAlgorithm) return null;

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
