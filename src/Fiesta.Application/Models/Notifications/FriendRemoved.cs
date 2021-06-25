using Fiesta.Domain.Entities.Notifications;
using Fiesta.Domain.Entities.Users;

namespace Fiesta.Application.Models.Notifications
{
    public class FriendRemoved : INotificationModel
    {
        public FriendRemoved(FiestaUser friend)
        {
            FriendId = friend.Id;
            FriendUsername = friend.Username;
            FriendPictureUrl = friend.PictureUrl;
        }

        public FriendRemoved()
        {
        }

        public string FriendId { get; set; }

        public string FriendUsername { get; set; }

        public string FriendPictureUrl { get; set; }

        public NotificationType Type => NotificationType.FriendRemoved;
    }
}
