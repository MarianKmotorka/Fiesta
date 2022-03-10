using Fiesta.Domain.Entities.Events;
using Fiesta.Domain.Entities.Notifications;

namespace Fiesta.Application.Models.Notifications
{
    public class EventJoinRequestCreatedNotification : INotificationModel
    {
        public EventJoinRequestCreatedNotification(EventJoinRequest joinRequest)
        {
            EventId = joinRequest.Event.Id;
            EventName = joinRequest.Event.Name;
            InterestedUserId = joinRequest.InterestedUser.Id;
            InterestedUserPictureUrl = joinRequest.InterestedUser.PictureUrl;
            InterestedUserUsername = joinRequest.InterestedUser.Username;
        }

        public EventJoinRequestCreatedNotification()
        {
        }

        public NotificationType Type => NotificationType.EventJoinRequestCreated;

        public string InterestedUserUsername { get; set; }

        public string InterestedUserId { get; set; }

        public string InterestedUserPictureUrl { get; set; }

        public string EventName { get; set; }

        public string EventId { get; set; }
    }
}
