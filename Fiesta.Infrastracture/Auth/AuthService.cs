﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Auth.GoogleLogin;
using Fiesta.Application.Common.Exceptions;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Options;
using Fiesta.Infrastracture.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Fiesta.Infrastracture.Auth
{
    public class AuthService : IAuthService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly FiestaDbContext _db;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public AuthService(JwtOptions jwtOptions, FiestaDbContext db, TokenValidationParameters tokenValidationParameters)
        {
            _jwtOptions = jwtOptions;
            _db = db;
            _tokenValidationParameters = tokenValidationParameters;
        }

        public async Task Logout(string refreshToken, CancellationToken cancellationToken)
        {
            var userId = GetPrincipalFromJwt(refreshToken)?.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

            if (userId is null)
                throw new BadRequestException("Invalid refresh token.");

            var user = await _db.Users.SingleOrNotFoundAsync(x => x.Id == userId, cancellationToken);

            if (refreshToken != user.RefreshToken)
                throw new BadRequestException("Invalid refresh token.");

            user.RefreshToken = null;
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<(string jwt, string refreshToken)> Login(GoogleUserInfoModel model, CancellationToken cancellationToken)
        {
            var user = await _db.Users.SingleOrDefaultAsync(x => x.Email.ToLower() == model.Email.ToLower(), cancellationToken)
               ?? CreateUser(model);

            return await CreateJwtAndRefreshToken(user, cancellationToken);
        }

        public async Task<(string jwt, string refreshToken)> RefreshJwt(string refreshToken, CancellationToken cancellationToken)
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
            var appUser = await _db.Users.SingleAsync(x => x.Id == appUserId);
            var storedRefreshToken = appUser.RefreshToken;

            if (storedRefreshToken != refreshToken)
                throw new BadRequestException("Invalid Refresh Token.");

            return await CreateJwtAndRefreshToken(appUser, cancellationToken);
        }

        private async Task<(string jwt, string refreshToken)> CreateJwtAndRefreshToken(AuthUser authUser, CancellationToken cancellationToken)
        {
            var accessToken = CreateAccessToken(authUser);
            var refreshToken = CreateRefreshToken(authUser);

            authUser.RefreshToken = refreshToken;
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
                new Claim(FiestaClaims.IsAccessToken,"true")
            };

            var refreshTokenObject = new JwtSecurityToken(
                _jwtOptions.Issuer,
                null,
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.Add(_jwtOptions.TokenLifeTime),
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
                );

            return tokenHandler.WriteToken(refreshTokenObject);
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

        private AuthUser CreateUser(GoogleUserInfoModel model)
        {
            var newUser = new AuthUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = model.Email,
                EmailConfirmed = model.IsEmailVerified,
            };

            _db.Users.Add(newUser);
            return newUser;
        }
    }
}