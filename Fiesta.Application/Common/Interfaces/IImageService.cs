using CloudinaryDotNet.Actions;
using Fiesta.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Fiesta.Application.Common.Interfaces
{
    public interface IImageService
    {
        Task<Result<RawUploadResult>> UploadProfilePictureToCloudinary(string userId, IFormFile formFile, CancellationToken cancellationToken);
    }
}
