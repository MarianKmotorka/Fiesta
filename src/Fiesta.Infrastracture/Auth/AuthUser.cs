using Fiesta.Application.Common.Constants;
using Microsoft.AspNetCore.Identity;
using System;

namespace Fiesta.Infrastracture.Auth
{
    public class AuthUser : IdentityUser<string>
    {
        public AuthUser(string email, AuthProviderEnum authProvider, string nickname) : this(email, FiestaRoleEnum.BasicUser, authProvider, nickname)
        {
        }

        public AuthUser(string email, FiestaRoleEnum role, AuthProviderEnum authProvider, string nickname)
        {
            Email = email.Trim().ToLower();
            UserName = Email;
            Role = role;
            AuthProvider = authProvider;
            Nickname = nickname;
            if (authProvider == AuthProviderEnum.Google)
                GoogleEmail = Email;
        }

        public string GoogleEmail { get; private set; }

        public string RefreshToken { get; set; }

        public string Nickname { get; set; }

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
