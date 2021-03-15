using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Interfaces;

namespace Fiesta.Application.Utils
{
    public static class AuthorizationCheckExtensions
    {
        public static bool IsResourceOwnerOrAdmin(this ICurrentUserService currentUserService, string resourceOwnerUserId)
        {
            return currentUserService.UserId == resourceOwnerUserId || currentUserService.Role == FiestaRoleEnum.Admin;
        }
    }
}
