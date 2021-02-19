using Fiesta.Application.Common.Constants;
using Microsoft.AspNetCore.Identity;

namespace Fiesta.Infrastracture.Auth
{
    public class AuthUser : IdentityUser<string>
    {
        public AuthUser(string email, AuthProviderEnum authProvider) : this(email, FiestaRoleEnum.BasicUser, authProvider)
        {
        }

        public AuthUser(string email, FiestaRoleEnum role, AuthProviderEnum authProvider)
        {
            Email = email.Trim().ToLower();
            UserName = Email;

            Role = role;
            AuthProvider = authProvider;
        }

        public string RefreshToken { get; set; }

        public AuthProviderEnum AuthProvider { get; set; }

        public FiestaRoleEnum Role { get; set; }
    }
}
