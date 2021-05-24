using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Features.Common;
using Fiesta.Application.Features.Notifications;
using Fiesta.Application.Models.Notifications;
using Fiesta.Application.Utils;
using Fiesta.Domain.Entities.Events;
using Fiesta.Domain.Entities.Notifications;
using Fiesta.Domain.Entities.Users;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.SignalR;
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
            private readonly IHubContext<NotificationsHub, INotificationsClient> _hub;

            public Handler(IFiestaDbContext db, IHubContext<NotificationsHub, INotificationsClient> hub)
            {
                _db = db;
                _hub = hub;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var @event = await _db.Events.Include(x => x.Organizer).SingleOrNotFoundAsync(x => x.Id == request.EventId, cancellationToken);
                var user = await _db.FiestaUsers.FindOrNotFoundAsync(cancellationToken, request.CurrentUserId);

                @event.AddJoinRequest(user);
                await _db.SaveChangesAsync(cancellationToken);

                var joinRequest = await _db.EventJoinRequests.FindOrNotFoundAsync(cancellationToken, new[] { request.EventId, request.CurrentUserId });
                await SendNotification(joinRequest, @event.Organizer, cancellationToken);

                return Unit.Value;
            }

            private async Task SendNotification(EventJoinRequest joinRequest, FiestaUser organizer, CancellationToken cancellationToken)
            {
                //TODO: Send to all permission holders
                var notificationModel = new EventJoinRequestCreatedNotification(joinRequest);
                var notification = _db.Notifications.Add(new Notification(organizer, notificationModel)).Entity;
                await _db.SaveChangesAsync(cancellationToken);

                await _hub.Notify(notification);
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
