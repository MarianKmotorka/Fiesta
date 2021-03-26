using System;

namespace Fiesta.Domain.Entities.Users
{
    public class UserFriend
    {
        public string UserId { get; private set; }
        public FiestaUser User { get; private set; }

        public string FriendId { get; private set; }
        public FiestaUser Friend { get; private set; }

        public UserFriend(FiestaUser user, FiestaUser friend)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            Friend = friend ?? throw new ArgumentNullException(nameof(friend));
            UserId = user.Id;
            FriendId = friend.Id;
        }

        private UserFriend()
        {

        }
    }
}
