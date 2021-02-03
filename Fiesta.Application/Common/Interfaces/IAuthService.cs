using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Auth.GoogleLogin;

namespace Fiesta.Application.Common.Interfaces
{
    public interface IAuthService
    {
        Task<(string accessToken, string refreshToken, bool authUserCreated)> LoginOrRegister(GoogleUserInfoModel model, CancellationToken cancellationToken);

        Task<(string accessToken, string refreshToken)> RefreshJwt(string refreshToken, CancellationToken cancellationToken);

        Task Logout(string refreshToken, CancellationToken cancellationToken);
    }
}
