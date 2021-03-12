using Fiesta.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Fiesta.Application.Common.Interfaces
{
    public interface IImageService
    {
        Task<Result> UploadImageToCloud(IFormFile picture, string filePath, CancellationToken cancellationToken);

        Task<Result> DeleteImageFromCloud(string filePath, CancellationToken cancellationToken);
    }
}
