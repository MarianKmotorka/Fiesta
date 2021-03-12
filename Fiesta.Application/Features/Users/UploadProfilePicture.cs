using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Exceptions;
using Fiesta.Application.Common.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Fiesta.Application.Features.Users
{
    public class UploadProfilePicture
    {
        public class Command : IRequest<Unit>
        {
            [JsonIgnore]
            public string UserId { get; set; }
            public IFormFile ProfilePicture { get; set; }
        }

        public class Handler : IRequestHandler<Command, Unit>
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

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var uploadResult = await _imageService.UploadImageToCloud(request.ProfilePicture, $"{CloudinaryFolders.ProfilePictures}/{request.UserId}", cancellationToken);

                if (uploadResult.Failed)
                    throw new BadRequestException(uploadResult.Errors);

                return Unit.Value;
            }

        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.ProfilePicture)
                    .Must(x => x.Length < 5000000).WithErrorCode(ErrorCodes.MaxSize).WithState(_ => new { MaxSize = 5000000 });

                RuleFor(x => x.ProfilePicture)
                    .Must(x => Path.GetExtension(x.FileName).ToLower() == ".jpg" || Path.GetExtension(x.FileName).ToLower() == ".png").WithErrorCode(ErrorCodes.UnsupportedMediaType);
            }
        }
    }
}
