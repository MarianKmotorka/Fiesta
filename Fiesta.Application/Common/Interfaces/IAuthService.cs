using Fiesta.Application.Auth;
using Fiesta.Application.Auth.GoogleLogin;
using System.Threading;
using System.Threading.Tasks;

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
    }
}
