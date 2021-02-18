using Fiesta.Application.Common.Constants;
using Microsoft.AspNetCore.Http;

namespace Fiesta.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        string UserId { get; }

        FiestaRoleEnum Role { get; }

        AuthProviderEnum AuthProvider { get; }

        HttpContext HttpContext { get; }
    }
}
