using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Features.Common;
using Fiesta.Application.Features.Events.Common;
using Fiesta.Application.Features.Notifications;
using Fiesta.Application.Models.Notifications;
using Fiesta.Domain.Entities.Events;
using Fiesta.Domain.Entities.Notifications;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Events
{
    public class DeleteEventAttendees
    {
        public class Command : IRequest
        {
            [JsonIgnore]
            public string EventId { get; set; }

            public List<string> RemoveUserIds { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IFiestaDbContext _db;
            private readonly ICurrentUserService _currentUser;
            private readonly IHubContext<NotificationsHub, INotificationsClient> _hub;

            public Handler(IFiestaDbContext db, ICurrentUserService currentUser, IHubContext<NotificationsHub, INotificationsClient> hub)
            {
                _db = db;
                _currentUser = currentUser;
                _hub = hub;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var attendees = await _db.EventAttendees
                    .Where(x => x.EventId == request.EventId && request.RemoveUserIds.Contains(x.AttendeeId))
                    .ToListAsync(cancellationToken);

                _db.EventAttendees.RemoveRange(attendees);
                await SendNotifications(attendees, request.EventId, cancellationToken);

                return Unit.Value;
            }

            private async Task SendNotifications(List<EventAttendee> attendees, string eventId, CancellationToken cancellationToken)
            {
                var @event = await _db.Events.Include(x => x.Organizer).SingleAsync(x => x.Id == eventId, cancellationToken);
                var currentUserLeftEvent = attendees.Count == 1 && _currentUser.UserId == attendees.Single().AttendeeId;

                if (currentUserLeftEvent)
                {
                    var user = await _db.FiestaUsers.FindAsync(new[] { _currentUser.UserId }, cancellationToken);
                    var notification = new Notification(@event.Organizer, new EventAttendeeLeft(@event, user));

                    _db.Notifications.Add(notification);
                    await _db.SaveChangesAsync(cancellationToken);

                    await _hub.Notify(notification);
                }
                else
                {
                    var createdNotifications = new List<Notification>();

                    foreach (var attendee in attendees)
                        createdNotifications.Add(new Notification(attendee.AttendeeId, new EventAttendeeRemoved(@event)));

                    _db.Notifications.AddRange(createdNotifications);
                    await _db.SaveChangesAsync(cancellationToken);

                    foreach (var notification in createdNotifications)
                        await _hub.Notify(notification);
                }
            }
        }

        public class AuthorizationCheck : IAuthorizationCheck<Command>
        {
            public async Task<bool> IsAuthorized(Command request, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken)
               => request.RemoveUserIds.Count == 1 &&
                  request.RemoveUserIds.Single() == currentUserService.UserId ||
                  await Helpers.IsOrganizerOrAdmin(request.EventId, db, currentUserService, cancellationToken);
        }
    }
}
