using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Exceptions;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Features.Events.Common;
using Fiesta.Application.Utils;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Fiesta.Application.Features.Events
{
    public class UploadEventBanner
    {
        public class Command : IRequest<Response>
        {
            public string EventId { get; set; }

            public IFormFile Banner { get; set; }
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
                var @event = await _db.Events.FindOrNotFoundAsync(cancellationToken, request.EventId);
                var uploadResult = await _imageService.Upload(request.Banner, CloudinaryPaths.EventBanner(request.EventId), cancellationToken);

                if (uploadResult.Failed)
                    throw new BadRequestException(uploadResult.Errors);

                @event.BannerUrl = uploadResult.Data;
                await _db.SaveChangesAsync(cancellationToken);

                return new() { BannerUrl = @event.BannerUrl };
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Banner)
                    .Cascade(CascadeMode.Stop)
                    .NotNull().WithErrorCode(ErrorCodes.Required)
                    .Must(x => x.Length < 500_000).WithErrorCode(ErrorCodes.MaxSize).WithState(_ => new { MaxSize = "500KB" })
                    .Must(x => x.ContentType.Split('/')[0] == "image").WithErrorCode(ErrorCodes.UnsupportedMediaType);
            }
        }

        public class AuthorizationCheck : IAuthorizationCheck<Command>
        {
            public async Task<bool> IsAuthorized(Command request, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken)
                => await Helpers.IsOrganizerOrAdmin(request.EventId, db, currentUserService, cancellationToken);
        }

        public class Response
        {
            public string BannerUrl { get; set; }
        }
    }
}
