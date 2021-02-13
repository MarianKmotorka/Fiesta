﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Auth;
using Fiesta.Application.Auth.GoogleLogin;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Exceptions;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Options;
using Fiesta.Infrastracture.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Fiesta.Infrastracture.Auth
{
    public class AuthService : IAuthService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly FiestaDbContext _db;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly UserManager<AuthUser> _userManager;

        public AuthService(JwtOptions jwtOptions, FiestaDbContext db, TokenValidationParameters tokenValidationParameters,
            UserManager<AuthUser> userManager)
        {
            _db = db;
            _jwtOptions = jwtOptions;
            _tokenValidationParameters = tokenValidationParameters;
            _userManager = userManager;
        }

        public async Task<(string accessToken, string refreshToken)> Login(string email, string password, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email.Trim());

            if (user is null)
                throw new BadRequestException(ErrorCodes.InvalidEmailOrPassword);

            if (user.AuthProvider != AuthProvider.EmailAndPassword)
                throw new BadRequestException(ErrorCodes.InvalidAuthProvider);

            var passValid = await _userManager.CheckPasswordAsync(user, password);

            if (!passValid)
                throw new BadRequestException(ErrorCodes.InvalidEmailOrPassword);

            if (!user.EmailConfirmed)
                throw new BadRequestException(ErrorCodes.EmailIsNotVerified);

            return await Login(user, cancellationToken);
        }

        public async Task Logout(string refreshToken, CancellationToken cancellationToken)
        {
            var userId = GetPrincipalFromJwt(refreshToken)?.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

            if (userId is null)
                throw new BadRequestException("Invalid refresh token.");

            var user = await _db.Users.SingleAsync(x => x.Id == userId, cancellationToken);

            if (refreshToken != user.RefreshToken)
                throw new BadRequestException("Invalid refresh token.");

            user.RefreshToken = null;
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<(string accessToken, string refreshToken, bool authUserCreated, string userId)> LoginOrRegister(GoogleUserInfoModel model, CancellationToken cancellationToken)
        {
            var user = await _db.Users.SingleOrDefaultAsync(x => x.Email == model.Email, cancellationToken);

            if (user is not null)
            {
                if (user.AuthProvider != AuthProvider.Google)
                    throw new BadRequestException(ErrorCodes.InvalidAuthProvider);

                var (accessToken, refreshToken) = await Login(user, cancellationToken);
                return (accessToken, refreshToken, false, user.Id);
            }

            var newUser = new AuthUser(model.Email, AuthProvider.Google) { EmailConfirmed = model.IsEmailVerified };

            _db.Users.Add(newUser);
            await _db.SaveChangesAsync(cancellationToken);

            var loginResult = await Login(newUser, cancellationToken);
            return (loginResult.accessToken, loginResult.refreshToken, true, newUser.Id);
        }

        public async Task<(string accessToken, string refreshToken)> RefreshJwt(string refreshToken, CancellationToken cancellationToken)
        {
            var validatedRefreshToken = GetPrincipalFromJwt(refreshToken);

            if (validatedRefreshToken?.Claims.SingleOrDefault(x => x.Type == FiestaClaims.IsRefreshToken) is null)
                throw new BadRequestException("Invalid Refresh Token");

            var expiryDateUnix =
                    long.Parse(validatedRefreshToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateUtc =
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expiryDateUnix);

            if (expiryDateUtc < DateTime.UtcNow)
                throw new BadRequestException("Refresh Token Is Expired.");

            var appUserId = validatedRefreshToken.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var appUser = await _db.Users.SingleAsync(x => x.Id == appUserId, cancellationToken);
            var storedRefreshToken = appUser.RefreshToken;

            if (storedRefreshToken != refreshToken)
                throw new BadRequestException("Invalid Refresh Token.");

            return await Login(appUser, cancellationToken);
        }

        public async Task<string> Register(RegisterWithEmailAndPassword.Command command, CancellationToken cancellationToken)
        {
            var newUser = new AuthUser(command.Email, AuthProvider.EmailAndPassword);

            var result = await _userManager.CreateAsync(newUser, command.Password);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.Select(x => x.Description));

            await _db.SaveChangesAsync(cancellationToken);
            return newUser.Id;
        }

        public async Task<string> GetEmailVerificationCode(string emailAddress, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(emailAddress);

            if (user is null)
                throw new BadRequestException(ErrorCodes.InvalidEmailAddress);
            if (user.EmailConfirmed)
                throw new BadRequestException(ErrorCodes.EmailAlreadyVerified);

            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task CheckEmailVerificationCode(string emailAddress, string code, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(emailAddress);

            if (user is null)
                throw new BadRequestException(ErrorCodes.InvalidEmailAddress);

            if (user.EmailConfirmed)
                throw new BadRequestException(ErrorCodes.EmailAlreadyVerified);

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (!result.Succeeded)
                throw new BadRequestException(ErrorCodes.InvalidCode);
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
