using Fiesta.Domain.Entities.Events;
using Fiesta.Domain.Entities.Notifications;

namespace Fiesta.Application.Models.Notifications
{
    public class EventJoinRequestReplyNotification : INotificationModel
    {
        public EventJoinRequestReplyNotification(EventJoinRequest joinRequest, bool accepted)
        {
            EventId = joinRequest.Event.Id;
            EventName = joinRequest.Event.Name;
            OrganizerId = joinRequest.Event.Organizer.Id;
            OrganizerPictureUrl = joinRequest.Event.Organizer.PictureUrl;
            OrganizerUsername = joinRequest.Event.Organizer.Username;
            Accepted = accepted;
        }

        public EventJoinRequestReplyNotification()
        {
        }

        public string EventId { get; set; }

        public string EventName { get; set; }

        public bool Accepted { get; set; }

        public string OrganizerUsername { get; set; }

        public string OrganizerPictureUrl { get; set; }

        public string OrganizerId { get; set; }

        public NotificationType Type => NotificationType.EventJoinRequestReply;
    }
}
