using Fiesta.Domain.Entities.Notifications;
using Fiesta.Domain.Entities.Users;

namespace Fiesta.Application.Models.Notifications
{
    public class FriendRequestReply : INotificationModel
    {
        public FriendRequestReply(FiestaUser friend, bool accepted)
        {
            FriendId = friend.Id;
            FriendUsername = friend.Username;
            FriendPictureUrl = friend.PictureUrl;
            Accepted = accepted;
        }

        public FriendRequestReply()
        {
        }

        public string FriendId { get; set; }

        public string FriendUsername { get; set; }

        public string FriendPictureUrl { get; set; }

        public bool Accepted { get; set; }

        public NotificationType Type => NotificationType.FriendRequestReply;
    }
}
