using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Models;
using Fiesta.Application.Features.Auth.CommonDtos;

namespace Fiesta.Application.Common.Interfaces
{
    public interface IGoogleService
    {
        Task<Result<GoogleUserInfoModel>> GetUserInfoModelForLogin(string code, CancellationToken cancellationToken);

        Task<Result<GoogleUserInfoModel>> GetUserInfoModelForConnectAccount(string code, CancellationToken cancellationToken);
    }
}
