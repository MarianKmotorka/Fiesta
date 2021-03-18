using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Models;
using Fiesta.Application.Features.Auth;
using Fiesta.Application.Features.Auth.CommonDtos;
using System.Threading;
using System.Threading.Tasks;

namespace Fiesta.Application.Common.Interfaces
{
    public interface IAuthService
    {
        Task<Result<(string accessToken, string refreshToken, bool authUserCreated, string userId, string username)>> LoginOrRegister(GoogleUserInfoModel model, CancellationToken cancellationToken);

        Task<Result<(string accessToken, string refreshToken)>> Login(string emailOrUsername, string password, CancellationToken cancellationToken);

        /// <summary>
        /// Registers new user and returns userId and username
        /// </summary>
        Task<Result<(string userId, string username)>> Register(RegisterWithEmailAndPassword.Command command, CancellationToken cancellationToken);

        Task<Result<(string accessToken, string refreshToken)>> RefreshJwt(string refreshToken, CancellationToken cancellationToken);

        Task<Result> Logout(string refreshToken, CancellationToken cancellationToken);

        Task<Result<string>> GetEmailVerificationCode(string emailAddress, CancellationToken cancellationToken);

        Task<Result> CheckEmailVerificationCode(string emailAddress, string code, CancellationToken cancellationToken);

        Task<Result> ResetPassword(string email, string token, string newPassword, CancellationToken cancellationToken);

        Task<Result<string>> GetResetPasswordToken(string email, CancellationToken cancellationToken);

        Task<Result> ChangePassword(string email, string currentPassword, string newPassword, CancellationToken cancellationToken);

        Task<AuthProviderEnum> GetAuthProvider(string userId, CancellationToken cancellationToken);

        Task<FiestaRoleEnum> GetRole(string userId, CancellationToken cancellationToken);

        Task<Result> AddPassword(string userId, string password, CancellationToken cancellationToken);

        Task<Result> AddGoogleAccount(string userId, GoogleUserInfoModel model, CancellationToken cancellationToken);

        Task<string> GetGoogleEmail(string userId, CancellationToken cancellationToken);

        Task<bool> IsEmailUnique(string email, CancellationToken cancellationToken);

        Task<Result> DeleteAccountWithPassword(string userId, string password, CancellationToken cancellationToken);

        Task<Result> DeleteAccountWithGoogle(string userId, GoogleUserInfoModel googleUser, CancellationToken cancellationToken);
    }
}
