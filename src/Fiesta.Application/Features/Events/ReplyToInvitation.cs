using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Events
{
    public class ReplyToInvitation
    {
        public class Command : IRequest
        {
            [JsonIgnore]
            public string EventId { get; set; }

            [JsonIgnore]
            public string CurrentUserId { get; set; }

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
                var invitation = await _db.EventInvitations
                    .SingleOrNotFoundAsync(x => x.EventId == request.EventId && x.InviteeId == request.CurrentUserId, cancellationToken);

                _db.EventInvitations.Remove(invitation);

                var joinRequest = await _db.EventJoinRequests
                    .SingleOrDefaultAsync(x => x.EventId == request.EventId && x.InterestedUserId == request.CurrentUserId, cancellationToken);

                if (joinRequest is not null)
                    _db.EventJoinRequests.Remove(joinRequest);

                if (request.Accepted)
                {
                    var invitedUser = await _db.FiestaUsers.FindOrNotFoundAsync(cancellationToken, request.CurrentUserId);
                    var @event = await _db.Events.FindOrNotFoundAsync(cancellationToken, request.EventId);
                    @event.AddAttendee(invitedUser);

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
    }
}
