using Fiesta.Domain.Entities.Events;
using Fiesta.Domain.Entities.Notifications;

namespace Fiesta.Application.Models.Notifications
{
    public class EventInvitationReplyNotification : INotificationModel
    {
        public EventInvitationReplyNotification(EventInvitation invitation, bool accepted)
        {
            EventId = invitation.Event.Id;
            EventName = invitation.Event.Name;
            InvitedId = invitation.Invitee.Id;
            InvitedUsername = invitation.Invitee.Username;
            Accepted = accepted;
        }

        public EventInvitationReplyNotification()
        {
        }

        public string EventId { get; set; }

        public string EventName { get; set; }

        public bool Accepted { get; set; }

        public string InvitedUsername { get; set; }

        public string InvitedId { get; set; }

        public NotificationType Type => NotificationType.EventInvitationReply;
    }
}
