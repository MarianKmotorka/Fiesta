using Microsoft.AspNetCore.Identity;

namespace Fiesta.Infrastracture.Auth
{
    public class AuthUser : IdentityUser<string>
    {
        public string RefreshToken { get; set; }

        public AuthProvider AuthProvider { get; set; }
    }

    public enum AuthProvider
    {
        None,
        Google,
        EmailAndPassword
    }
}
