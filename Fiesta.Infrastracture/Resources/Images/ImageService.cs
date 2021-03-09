using CloudinaryDotNet;
using Fiesta.Application.Common.Options;
using System.Collections.Generic;
using System.Diagnostics;

namespace Fiesta.Infrastracture.Resources.Images
{
    public class ImageService
    {
        private readonly CloudinaryOptions _cloudinaryOptions;

        public ImageService(CloudinaryOptions cloudinaryOptions)
        {
            _cloudinaryOptions = cloudinaryOptions;
        }

        private SignatureResponse GenerateCloudinarySignature()
        {
            var timestamp = Stopwatch.GetTimestamp().ToString();

            var cloudinary = new Cloudinary(
                new Account(
                    _cloudinaryOptions.CloudName,
                    _cloudinaryOptions.ApiKey,
                    _cloudinaryOptions.ApiSecret
                    )
                );

            var parameters = new Dictionary<string, object>() { ["timestamp"] = timestamp };
            var signature = cloudinary.Api.SignParameters(parameters);

            return new SignatureResponse
            {
                Timestamp = timestamp,
                Signature = signature
            };

        }

        public class SignatureResponse
        {
            public string Timestamp { get; set; }

            public string Signature { get; set; }
        }
    }
}
