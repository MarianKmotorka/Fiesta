using System;
using System.Linq;
using System.Security.Claims;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Fiesta.Infrastracture.Auth
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            HttpContext = httpContextAccessor.HttpContext;
            var userClaims = HttpContext?.User.Claims;

            var roleString = userClaims?.SingleOrDefault(x => x.Type == FiestaClaims.FiestaRole)?.Value;
            Enum.TryParse<FiestaRoleEnum>(roleString, out var roleEnum);

            UserId = userClaims?.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            Role = roleEnum;
        }

        public string UserId { get; }

        public FiestaRoleEnum Role { get; }

        public AuthProviderEnum AuthProvider { get; }

        public HttpContext HttpContext { get; }
    }
}
