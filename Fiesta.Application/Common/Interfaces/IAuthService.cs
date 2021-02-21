﻿using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Features.Auth;
using Fiesta.Application.Features.Auth.GoogleLogin;

namespace Fiesta.Application.Common.Interfaces
{
    public interface IAuthService
    {
        Task<(string accessToken, string refreshToken, bool authUserCreated, string userId)> LoginOrRegister(GoogleUserInfoModel model, CancellationToken cancellationToken);

        Task<(string accessToken, string refreshToken)> Login(string email, string password, CancellationToken cancellationToken);

        /// <summary>
        /// Registers new user and returns userId
        /// </summary>
        Task<string> Register(RegisterWithEmailAndPassword.Command command, CancellationToken cancellationToken);

        Task<(string accessToken, string refreshToken)> RefreshJwt(string refreshToken, CancellationToken cancellationToken);

        Task Logout(string refreshToken, CancellationToken cancellationToken);

        Task<string> GetEmailVerificationCode(string emailAddress, CancellationToken cancellationToken);

        Task CheckEmailVerificationCode(string emailAddress, string code, CancellationToken cancellationToken);

        Task ResetPassword(string email, string token, string newPassword, CancellationToken cancellationToken);

        Task<string> GetResetPasswordToken(string email, CancellationToken cancellationToken);

        Task ChangePassword(string email, string currentPassword, string newPassword, CancellationToken cancellationToken);

        Task<AuthProviderEnum> GetAuthProvider(string id);

        Task<FiestaRoleEnum> GetRole(string id);

        Task DeleteAccountWithPassword(string userId, string password, CancellationToken cancellationToken);
    }
}
