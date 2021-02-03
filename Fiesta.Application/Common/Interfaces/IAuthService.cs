using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Auth.GoogleLogin;

namespace Fiesta.Application.Common.Interfaces
{
    public interface IAuthService
    {
        Task<(string jwt, string refreshToken)> Login(GoogleUserInfoModel model, CancellationToken cancellationToken);

        Task<(string jwt, string refreshToken)> RefreshJwt(string refreshToken, CancellationToken cancellationToken);

        Task Logout(string refreshToken, CancellationToken cancellationToken);
    }
}
