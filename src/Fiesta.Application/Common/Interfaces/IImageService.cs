using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Models;
using Microsoft.AspNetCore.Http;

namespace Fiesta.Application.Common.Interfaces
{
    public interface IImageService
    {
        Task<Result<string>> Upload(IFormFile picture, string filePath, CancellationToken cancellationToken);

        Task<Result> Delete(string filePath, CancellationToken cancellationToken);

        string Domain { get; }
    }
}
