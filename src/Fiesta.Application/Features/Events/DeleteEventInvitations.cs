using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Features.Events.Common;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Events
{
    public class DeleteEventInvitations
    {
        public class Command : IRequest
        {
            [JsonIgnore]
            public string CurrentUserId { get; set; }

            [JsonIgnore]
            public string EventId { get; set; }

            public List<string> RemoveUserIds { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var removeInvitations = await _db.EventInvitations
                    .Where(x => x.EventId == request.EventId && request.RemoveUserIds.Contains(x.InviteeId))
                    .ToListAsync(cancellationToken);

                _db.EventInvitations.RemoveRange(removeInvitations);

                await _db.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.RemoveUserIds).NotEmpty().WithErrorCode(ErrorCodes.Required);
            }
        }

        public class AuthorizationCheck : IAuthorizationCheck<Command>
        {
            public async Task<bool> IsAuthorized(Command request, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken)
                => await Helpers.IsOrganizerOrAdmin(request.EventId, db, currentUserService, cancellationToken);

        }
    }
}
