using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Events
{
    public class ReplyToEventJoinRequest
    {
        public class Command : IRequest
        {
            [JsonIgnore]
            public string EventId { get; set; }

            public string UserId { get; set; }

            public bool Accepted { get; set; }
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
                var joinRequest = await _db.EventJoinRequests
                    .SingleOrNotFoundAsync(x => x.EventId == request.EventId && x.InterestedUserId == request.UserId, cancellationToken);

                _db.EventJoinRequests.Remove(joinRequest);

                var invitation = await _db.EventInvitations
                    .SingleOrDefaultAsync(x => x.EventId == request.EventId && x.InviteeId == request.UserId, cancellationToken);

                if (invitation is not null)
                    _db.EventInvitations.Remove(invitation);

                if (request.Accepted)
                {
                    var requestedUser = await _db.FiestaUsers.FindOrNotFoundAsync(cancellationToken, request.UserId);
                    var @event = await _db.Events.FindOrNotFoundAsync(cancellationToken, request.EventId);
                    @event.AddAttendee(requestedUser);

                    //TODO: Send accepted notification
                }
                else
                {
                    //TODO: SEnd rejected notification
                }

                await _db.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }

        public class AuthorizationCheck : IAuthorizationCheck<Command>
        {
            public async Task<bool> IsAuthorized(Command request, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken)
            {
                var @event = await db.Events.FindOrNotFoundAsync(cancellationToken, request.EventId);
                return @event.OrganizerId == currentUserService.UserId;
            }
        }
    }
}
