using Fiesta.Application.Common.Constants;
using Microsoft.AspNetCore.Identity;
using System;

namespace Fiesta.Infrastracture.Auth
{
    public class AuthUser : IdentityUser<string>
    {
        public AuthUser(string email, AuthProviderEnum authProvider, string userName) : this(email, FiestaRoleEnum.BasicUser, authProvider, userName)
        {
        }

        public AuthUser(string email, FiestaRoleEnum role, AuthProviderEnum authProvider, string userName)
        {
            Email = email.Trim().ToLower();
            Role = role;
            AuthProvider = authProvider;
            UserName = userName;
            if (authProvider == AuthProviderEnum.Google)
                GoogleEmail = Email;
        }

        public string GoogleEmail { get; private set; }

        public string RefreshToken { get; set; }

        public AuthProviderEnum AuthProvider { get; private set; }

        public FiestaRoleEnum Role { get; private set; }

        public void AddGoogleAuthProvider(string googleEmail)
        {
            if (AuthProvider.HasFlag(AuthProviderEnum.Google))
                throw new InvalidOperationException("User already has a google account");

            GoogleEmail = googleEmail.ToLower();
            AuthProvider |= AuthProviderEnum.Google;
        }

        public void AddEmailAndPasswordAuthProvider()
        {
            AuthProvider |= AuthProviderEnum.EmailAndPassword;
        }
    }
}
