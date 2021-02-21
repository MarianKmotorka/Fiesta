using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Fiesta.Application.Common.Options;
using Fiesta.Infrastracture;
using Fiesta.Infrastracture.Auth;
using Microsoft.IdentityModel.Tokens;

namespace Fiesta.IntegrationTests.Helpers
{
    public static class AuthHelpers
    {
        public static string GetAccessToken(this AuthUser user, FiestaAppFactory factory)
        {
            var options = factory.Services.GetService(typeof(JwtOptions)) as JwtOptions;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(options.Secret);
            var jti = Guid.NewGuid().ToString();
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti,  jti),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Iss, options.Issuer),
                new Claim(FiestaClaims.FiestaRole, user.Role.ToString()),
                new Claim(FiestaClaims.IsAccessToken,"true")
            };

            var accessTokenObject = new JwtSecurityToken(
                options.Issuer,
                null,
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.Add(options.TokenLifeTime),
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
                );

            return tokenHandler.WriteToken(accessTokenObject);
        }
    }
}
