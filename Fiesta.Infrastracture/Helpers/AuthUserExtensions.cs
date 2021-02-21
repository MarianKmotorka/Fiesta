using System.Linq;
using Fiesta.Infrastracture.Auth;

namespace Fiesta.Infrastracture.Helpers
{
    public static class AuthUserExtensions
    {
        public static IQueryable<AuthUser> WhereSomeEmailIs(this IQueryable<AuthUser> query, string email)
        {
            return query.Where(x => x.Email == email || x.GoogleEmail == email);
        }
    }
}
