using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Exceptions;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Utils;
using MediatR;

namespace Fiesta.Application.Features.Users
{
    public class DeleteProfilePicture
    {
        public class Command : IRequest<Unit>
        {
            [JsonIgnore]
            public string UserId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly IFiestaDbContext _db;
            private readonly IImageService _imageService;

            public Handler(IFiestaDbContext db, IImageService imageService)
            {
                _db = db;
                _imageService = imageService;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var uploadResult = await _imageService.Delete(CloudinaryPaths.ProfilePicture(request.UserId), cancellationToken);

                if (uploadResult.Failed)
                    throw new BadRequestException(uploadResult.Errors);

                var fiestaUser = await _db.FiestaUsers.FindOrNotFoundAsync(cancellationToken, request.UserId);

                fiestaUser.PictureUrl = null;
                await _db.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }

            public class AuthorizationCheck : IAuthorizationCheck<Command>
            {
                public Task<bool> IsAuthorized(Command request, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken)
                    => Task.FromResult(currentUserService.IsResourceOwnerOrAdmin(request.UserId));
            }
        }
    }
}
