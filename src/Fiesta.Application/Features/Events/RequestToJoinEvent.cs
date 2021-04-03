using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Utils;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Events
{
    public class RequestToJoinEvent
    {
        public class Command : IRequest
        {
            public string CurrentUserId { get; set; }

            public string EventId { get; set; }
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
                var @event = await _db.Events.FindOrNotFoundAsync(cancellationToken, request.EventId);
                var user = await _db.FiestaUsers.FindOrNotFoundAsync(cancellationToken, request.CurrentUserId);

                @event.AddJoinRequest(user);
                await _db.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly IFiestaDbContext _db;

            public Validator(IFiestaDbContext db)
            {
                _db = db;

                RuleFor(x => x.EventId)
                    .MustAsync(NotInvitedOrAttendee).WithErrorCode(ErrorCodes.AlreadyAttendeeOrInvited);
            }

            private async Task<bool> NotInvitedOrAttendee(Command command, string eventId, CancellationToken cancellationToken)
            {
                var @event = await _db.Events.FindOrNotFoundAsync(cancellationToken, eventId);
                if (@event.OrganizerId == command.CurrentUserId)
                    return false;

                var alreadyAttendee = await _db.Events.Where(x => x.Id == command.EventId)
                    .SelectMany(x => x.Attendees.Select(a => a.AttendeeId))
                    .AnyAsync(x => x == command.CurrentUserId, cancellationToken);

                return !alreadyAttendee;
            }
        }
    }
}
