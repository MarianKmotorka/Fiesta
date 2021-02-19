using Fiesta.Application.Common.Constants;
using Microsoft.AspNetCore.Http;

namespace Fiesta.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        string UserId { get; }

        FiestaRoleEnum Role { get; }

        HttpContext HttpContext { get; }
    }
}
