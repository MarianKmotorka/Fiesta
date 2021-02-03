using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CloudinaryDotNet;
using Fiesta.Application.Common.Options;
using MediatR;

namespace Fiesta.Application.Auth
{
    public class GetCloudinarySignature
    {
        public class Query : IRequest<Response>
        {
        }

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly CloudinaryOptions _cloudinaryOptions;

            public Handler(CloudinaryOptions cloudinaryOptions)
            {
                _cloudinaryOptions = cloudinaryOptions;
            }

            public Task<Response> Handle(Query request, CancellationToken cancellationToken)
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

                return Task.FromResult(new Response
                {
                    Timestamp = timestamp,
                    Signature = signature
                });

            }
        }

        public class Response
        {
            public string Timestamp { get; set; }

            public string Signature { get; set; }
        }
    }
}
