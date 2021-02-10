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
        public static readonly IReadOnlyDictionary<FiestaRole, IEnumerable<FiestaRole>> Map = new Dictionary<FiestaRole, IEnumerable<FiestaRole>>
        {
            [FiestaRole.BasicUser] = new[] { FiestaRole.BasicUser },
            [FiestaRole.PremiumUser] = new[] { FiestaRole.PremiumUser, FiestaRole.BasicUser },
            [FiestaRole.Admin] = new[] { FiestaRole.Admin, FiestaRole.PremiumUser, FiestaRole.BasicUser },
        };
    }
}
