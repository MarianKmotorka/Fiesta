using System.Linq;
using System.Security.Claims;
using Fiesta.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Fiesta.Infrastracture.Auth
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            HttpContext = httpContextAccessor.HttpContext;
            var userClaims = httpContextAccessor.HttpContext?.User.Claims;

            UserId = userClaims?.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public string UserId { get; }

        public HttpContext HttpContext { get; }
    }
}
