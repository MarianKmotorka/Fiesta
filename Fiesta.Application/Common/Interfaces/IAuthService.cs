using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Auth;
using Fiesta.Application.Auth.GoogleLogin;

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
    }
}
