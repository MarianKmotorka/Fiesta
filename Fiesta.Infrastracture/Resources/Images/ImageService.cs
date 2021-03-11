using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
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
    public class ImageService : IImageService
    {
        private readonly CloudinaryOptions _cloudinaryOptions;
        private readonly Cloudinary _cloudinary;

        public ImageService(CloudinaryOptions cloudinaryOptions)
        {
            _cloudinaryOptions = cloudinaryOptions;
            _cloudinary = new Cloudinary(
                new Account(
                    _cloudinaryOptions.CloudName,
                    _cloudinaryOptions.ApiKey,
                    _cloudinaryOptions.ApiSecret
                    )
                );
        }

        private SignatureResponse GenerateCloudinarySignature()
        {
            var timestamp = Stopwatch.GetTimestamp().ToString();

            var parameters = new Dictionary<string, object>() { ["timestamp"] = timestamp };
            var signature = _cloudinary.Api.SignParameters(parameters);

            return new SignatureResponse
            {
                TimeStamp = timestamp,
                Signature = signature
            };

        }

        public async Task<Result<RawUploadResult>> UploadFileToCloudinary(string folder, IFormFile formFile, CancellationToken cancellationToken, string publicId = "", string fileType = "image", bool overwrite = true)
        {
            GenerateCloudinarySignature();
            var file = new FileDescription(formFile.FileName, formFile.OpenReadStream());

            var parameters = new Dictionary<string, object>()
            {
                ["async"] = true,
                ["folder"] = folder,
            };

            if (string.IsNullOrEmpty(publicId))
            {
                parameters.Add("use_filename", true);
                parameters.Add("overwrite", true);
                parameters.Add("unique_filename", false);
            }
            else
            {
                parameters.Add("public_id", publicId);
                parameters.Add("overwrite", overwrite);
            }

            var result = await _cloudinary.UploadAsync(fileType, parameters, file, cancellationToken);

            if (result.StatusCode != HttpStatusCode.OK)
                return Result<RawUploadResult>.Failure(result.Error.Message);

            return Result<RawUploadResult>.Success(result);
        }

        public class SignatureResponse
        {
            public string TimeStamp { get; set; }

            public string Signature { get; set; }
        }
    }
}
