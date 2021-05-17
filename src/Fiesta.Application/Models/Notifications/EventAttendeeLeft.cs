using Fiesta.Domain.Entities.Events;
using Fiesta.Domain.Entities.Notifications;
using Fiesta.Domain.Entities.Users;

namespace Fiesta.Application.Models.Notifications
{
    public class EventAttendeeLeft : INotificationModel
    {
        public EventAttendeeLeft(Event @event, FiestaUser attendee)
        {
            EventId = @event.Id;
            EventName = @event.Name;
            AttendeeId = attendee.Id;
            AttendeeUsername = attendee.Username;
            AttendeePictureUrl = attendee.PictureUrl;
        }

        public EventAttendeeLeft()
        {
        }

        public NotificationType Type => NotificationType.EventAttendeeLeft;

        public string EventName { get; set; }

        public string EventId { get; set; }

        public string AttendeeUsername { get; set; }

        public string AttendeeId { get; set; }

        public string AttendeePictureUrl { get; set; }
    }
}
