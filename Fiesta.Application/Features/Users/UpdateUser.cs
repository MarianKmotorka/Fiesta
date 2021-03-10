using CloudinaryDotNet.Actions;
using Fiesta.Application.Common.Exceptions;
using Fiesta.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Fiesta.Application.Features.Users
{
    public class UpdateUser
    {
        public class Query : IRequest<Response>
        {
            [JsonIgnore]
            public string UserId { get; set; }
            public IFormFile FormFile { get; set; }
        }

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly IFiestaDbContext _db;
            private readonly IAuthService _authService;
            private readonly IImageService _imageService;

            public Handler(IFiestaDbContext db, IAuthService authService, IImageService imageService)
            {
                _db = db;
                _authService = authService;
                _imageService = imageService;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                var uploadResult = await _imageService.UploadProfilePictureToCloudinary(request.UserId, request.FormFile, cancellationToken);

                if (!uploadResult.Failed)
                    throw new BadRequestException(uploadResult.Errors);

                return new Response() { ProfilePictureResponse = uploadResult.Data };
            }

        }

        public class Response
        {
            public RawUploadResult ProfilePictureResponse { get; set; }
        }
    }
}
