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
        public class Query : IRequest<string>
        {
            [JsonIgnore]
            public string UserId { get; set; }
            public IFormFile ProfilePicture { get; set; }
        }

        public class Handler : IRequestHandler<Query, string>
        {
            private readonly IFiestaDbContext _db;
            private readonly IImageService _imageService;

            public Handler(IFiestaDbContext db, IImageService imageService)
            {
                _db = db;
                _imageService = imageService;
            }

            public async Task<string> Handle(Query request, CancellationToken cancellationToken)
            {
                var uploadResult = await _imageService.UploadImageToCloud(request.ProfilePicture, $"{CloudinaryFolders.ProfilePictures}/{request.UserId}", cancellationToken);

                if (uploadResult.Failed)
                    throw new BadRequestException(uploadResult.Errors);

                var fiestaUser = await _db.FiestaUsers.FindAsync(new[] { request.UserId }, cancellationToken);

                fiestaUser.PictureUrl = uploadResult.Data;
                await _db.SaveChangesAsync(cancellationToken);

                return uploadResult.Data;
            }
        }

        public class Validator : AbstractValidator<Query>
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
