using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Models;
using Fiesta.Application.Common.Options;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Diagnostics;
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

        public async Task<Result> UploadImageToCloud(IFormFile picture, string filePath, CancellationToken cancellationToken)
        {
            var result = await UploadFileToCloudinary(picture, filePath, CloudinaryFileTypes.Image, cancellationToken);

            if (result.StatusCode == HttpStatusCode.OK)
                return Result.Success();
            else
                return Result.Failure(result.Error.Message);
        }

        private async Task<RawUploadResult> UploadFileToCloudinary(IFormFile formFile, string filePath, string fileType, CancellationToken cancellationToken, bool overwrite = true)
        {
            GenerateCloudinarySignature();
            var file = new FileDescription(formFile.FileName, formFile.OpenReadStream());

            var parameters = new Dictionary<string, object>()
            {
                ["async"] = true,
                ["public_id"] = filePath,
                ["overwrite"] = overwrite,
            };

            return await _cloudinary.UploadAsync(fileType, parameters, file, cancellationToken);
        }

        private void GenerateCloudinarySignature()
        {
            var timestamp = Stopwatch.GetTimestamp().ToString();
            var parameters = new Dictionary<string, object>() { ["timestamp"] = timestamp };
            _cloudinary.Api.SignParameters(parameters);
        }
    }
}
