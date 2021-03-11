using CloudinaryDotNet.Actions;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Fiesta.Application.Common.Interfaces
{
    public interface IImageService
    {
        /// <summary>
        /// Uploads file to Cloudinary
        /// </summary>
        /// <param name="folder">Name of the folder where the file should be saved</param>
        /// <param name="formFile">Bytes and name of file</param>
        /// <param name="fileType">Type of file. Valid values: image, raw, video, auto</param>
        /// <param name="overwrite">Determines whether the file with the same params should be overwritten</param>
        /// <param name="publicId">Public identifier of file (e.g. user ID for profile picture)</param>
        Task<Result<RawUploadResult>> UploadFileToCloudinary(string folder, IFormFile formFile, CancellationToken cancellationToken, string publicId = "", string fileType = CloudinaryFileTypes.Image, bool overwrite = true);
    }
}
