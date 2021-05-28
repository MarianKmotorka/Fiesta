using Fiesta.Domain.Entities.Events;
using Fiesta.Domain.Entities.Notifications;
using Fiesta.Domain.Entities.Users;

namespace Fiesta.Application.Models.Notifications
{
    public class EventJoinRequestReplyNotification : INotificationModel
    {
        public EventJoinRequestReplyNotification(EventJoinRequest joinRequest, FiestaUser responder, bool accepted)
        {
            EventId = joinRequest.Event.Id;
            EventName = joinRequest.Event.Name;
            ResponderId = responder.Id;
            ResponderPictureUrl = responder.PictureUrl;
            ResponderUsername = responder.Username;
            Accepted = accepted;
        }

        public EventJoinRequestReplyNotification()
        {
        }

        public string EventId { get; set; }

        public string EventName { get; set; }

        public bool Accepted { get; set; }

        public string ResponderUsername { get; set; }

        public string ResponderPictureUrl { get; set; }

        public string ResponderId { get; set; }

        public NotificationType Type => NotificationType.EventJoinRequestReply;
    }
}
