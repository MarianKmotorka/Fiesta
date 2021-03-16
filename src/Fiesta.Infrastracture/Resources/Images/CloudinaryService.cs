using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Models;
using Fiesta.Application.Common.Options;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Fiesta.Infrastracture.Resources.Images
{
    public class CloudinaryService : IImageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(CloudinaryOptions cloudinaryOptions)
        {
            _cloudinary = new Cloudinary(
                new Account(
                    cloudinaryOptions.CloudName,
                    cloudinaryOptions.ApiKey,
                    cloudinaryOptions.ApiSecret
                    )
                );
        }

        public async Task<Result<string>> UploadImageToCloud(IFormFile picture, string filePath, CancellationToken cancellationToken)
        {
            var result = await UploadFileToCloudinary(picture, filePath, CloudinaryFileTypes.Image, cancellationToken);

            if (result.StatusCode == HttpStatusCode.OK)
                return Result<string>.Success(result.Url.OriginalString);
            else
                return Result<string>.Failure(result.Error.Message);
        }

        public async Task<Result> DeleteImageFromCloud(string filePath, CancellationToken cancellationToken)
        {
            var result = await _cloudinary.DeleteResourcesAsync
                (
                new DelResParams()
                {
                    ResourceType = ResourceType.Image,
                    PublicIds = new List<string>() { filePath }
                },
                cancellationToken
                );

            if (result.StatusCode == HttpStatusCode.OK)
                return Result.Success();
            else
                return Result.Failure(result.Error.Message);
        }

        private async Task<RawUploadResult> UploadFileToCloudinary(IFormFile formFile, string filePath, string fileType, CancellationToken cancellationToken, bool overwrite = true)
        {
            var file = new FileDescription(formFile.FileName, formFile.OpenReadStream());

            var parameters = new Dictionary<string, object>()
            {
                ["public_id"] = filePath,
                ["overwrite"] = overwrite,
            };

            return await _cloudinary.UploadAsync(fileType, parameters, file, cancellationToken);
        }
    }
}
