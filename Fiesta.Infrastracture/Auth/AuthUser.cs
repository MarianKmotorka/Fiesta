using Fiesta.Application.Common.Constants;
using Microsoft.AspNetCore.Identity;

namespace Fiesta.Infrastracture.Auth
{
    public class AuthUser : IdentityUser<string>
    {
        public AuthUser(string email, AuthProvider authProvider) : this(email, FiestaRole.BasicUser, authProvider)
        {
        }

        public AuthUser(string email, FiestaRole role, AuthProvider authProvider)
        {
            Email = email.Trim().ToLower();
            UserName = Email;

            Role = role;
            AuthProvider = authProvider;
        }

        public string RefreshToken { get; set; }

        public AuthProvider AuthProvider { get; set; }

        public FiestaRole Role { get; set; }
    }
}
