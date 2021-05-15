using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Options;
using Fiesta.Infrastracture.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Fiesta.Infrastracture.DependencyInjection
{
    public static class AuthDIExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtOptions = new JwtOptions();
            configuration.GetSection(nameof(JwtOptions)).Bind(jwtOptions);
            services.AddSingleton(jwtOptions);

            var oAuthOptions = new GoogleOAuthOptions();
            configuration.GetSection(nameof(GoogleOAuthOptions)).Bind(oAuthOptions);
            services.AddSingleton(oAuthOptions);

            var tokenValidationParams = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddSingleton(tokenValidationParams);

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = tokenValidationParams;

                o.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var isAccessTokenClaim = context.Principal.Claims.SingleOrDefault(x => x.Type == FiestaClaims.IsAccessToken);
                        if (isAccessTokenClaim is null)
                            context.Fail(new Exception("Token is missing 'IsAccessToken' claim."));

                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(accessToken))
                            context.Token = accessToken;

                        return Task.CompletedTask;
                    }
                };

                o.SaveToken = true;
            });

            services.AddTransient<IAuthService, AuthService>();
            return services;
        }
    }
}
