using Fiesta.Domain.Entities.Events;
using Fiesta.Domain.Entities.Notifications;

namespace Fiesta.Application.Models.Notifications
{
    public class EventInvitationCreatedNotification : INotificationModel
    {
        public EventInvitationCreatedNotification(EventInvitation invitation)
        {
            EventId = invitation.Event.Id;
            EventName = invitation.Event.Name;
            InviterId = invitation.Inviter.Id;
            InviterPictureUrl = invitation.Inviter.PictureUrl;
            InviterUsername = invitation.Inviter.Username;
        }

        public EventInvitationCreatedNotification()
        {
        }

        public NotificationType Type => NotificationType.EventInvitationCreated;

        public string InviterUsername { get; set; }

        public string InviterId { get; set; }

        public string InviterPictureUrl { get; set; }

        public string EventName { get; set; }

        public string EventId { get; set; }
    }
}
