using System;
using System.Linq;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Utils;
using Fiesta.Infrastracture;
using Microsoft.Extensions.DependencyInjection;

namespace Fiesta.WebApi.Extensions
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddFiestaAuthorization(this IServiceCollection services)
        {
            var roleNames = RolesHelper.Map.Keys.Select(x => x.ToString());

            services.AddAuthorization(options =>
            {
                foreach (var requiredRole in roleNames)
                {
                    options.AddPolicy(requiredRole, x => x.RequireAssertion(ctx =>
                    {
                        var userRole = ctx.User.Claims.Single(x => x.Type == FiestaClaims.FiestaRole).Value;

                        if (!Enum.TryParse<FiestaRole>(userRole, out var userRoleEnum))
                            return false;

                        return RolesHelper.Map[userRoleEnum].Any(x => x.ToString() == requiredRole);
                    }));
                }
            });

            return services;
        }
    }
}
