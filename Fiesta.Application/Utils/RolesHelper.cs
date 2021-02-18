using System.Collections.Generic;
using Fiesta.Application.Common.Constants;

namespace Fiesta.Application.Utils
{
    public static class RolesHelper
    {
        /// <summary>
        /// Maps each ROLE to all ROLES that it contains
        /// </summary>
        /// <remarks>
        /// E.g.: PremiumUser can access features made for PremiumUser as well as features for BasicUser
        /// </remarks>
        public static readonly IReadOnlyDictionary<FiestaRoleEnum, IEnumerable<FiestaRoleEnum>> Map = new Dictionary<FiestaRoleEnum, IEnumerable<FiestaRoleEnum>>
        {
            [FiestaRoleEnum.BasicUser] = new[] { FiestaRoleEnum.BasicUser },
            [FiestaRoleEnum.PremiumUser] = new[] { FiestaRoleEnum.PremiumUser, FiestaRoleEnum.BasicUser },
            [FiestaRoleEnum.Admin] = new[] { FiestaRoleEnum.Admin, FiestaRoleEnum.PremiumUser, FiestaRoleEnum.BasicUser },
        };
    }
}
