using Fiesta.Domain.Entities.Events;
using Fiesta.Domain.Entities.Notifications;

namespace Fiesta.Application.Models.Notifications
{
    public class EventAttendeeRemoved : INotificationModel
    {
        public EventAttendeeRemoved(Event @event)
        {
            EventId = @event.Id;
            EventName = @event.Name;
            RemovedById = @event.OrganizerId;
            RemovedByUsername = @event.Organizer.Username;
            RemovedByPictureUrl = @event.Organizer.PictureUrl;
        }

        public EventAttendeeRemoved()
        {
        }

        public NotificationType Type => NotificationType.EventAttendeeRemoved;

        public string EventId { get; set; }

        public string EventName { get; set; }

        public string RemovedById { get; set; }

        public string RemovedByUsername { get; set; }

        public string RemovedByPictureUrl { get; set; }
    }
}
