using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Models;
using Fiesta.Application.Features.Auth.CommonDtos;

namespace Fiesta.Application.Common.Interfaces
{
    public interface IGoogleService
    {
        Task<Result<GoogleUserInfoModel>> GetUserInfoModel(string code, CancellationToken cancellationToken);
    }
}
