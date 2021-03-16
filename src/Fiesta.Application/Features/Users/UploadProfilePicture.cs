using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Exceptions;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Utils;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Fiesta.Application.Features.Users
{
    public class UploadProfilePicture
    {
        public class Command : IRequest<Response>
        {
            public string UserId { get; set; }
            public IFormFile ProfilePicture { get; set; }
        }

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly IFiestaDbContext _db;
            private readonly IImageService _imageService;

            public Handler(IFiestaDbContext db, IImageService imageService)
            {
                _db = db;
                _imageService = imageService;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                var uploadResult = await _imageService.UploadImageToCloud(request.ProfilePicture, $"{CloudinaryFolders.ProfilePictures}/{request.UserId}", cancellationToken);

                if (uploadResult.Failed)
                    throw new BadRequestException(uploadResult.Errors);

                var fiestaUser = await _db.FiestaUsers.FindOrNotFoundAsync(cancellationToken, request.UserId);

                fiestaUser.PictureUrl = uploadResult.Data;
                await _db.SaveChangesAsync(cancellationToken);

                return new Response { PictureUrl = uploadResult.Data };
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.ProfilePicture)
                    .Cascade(CascadeMode.Stop)
                    .NotNull().WithErrorCode(ErrorCodes.Required)
                    .Must(x => x.Length < 500_000).WithErrorCode(ErrorCodes.MaxSize).WithState(_ => new { MaxSize = "500KB" })
                    .Must(x => x.ContentType.Split('/')[0] == "image").WithErrorCode(ErrorCodes.UnsupportedMediaType);
            }
        }

        public class AuthorizationCheck : IAuthorizationCheck<Command>
        {
            public Task<bool> IsAuthorized(Command request, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken)
                => Task.FromResult(currentUserService.IsResourceOwnerOrAdmin(request.UserId));
        }

        public class Response
        {
            public string PictureUrl { get; set; }
        }
    }
}
