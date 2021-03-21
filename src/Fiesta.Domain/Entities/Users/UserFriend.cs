using System;

namespace Fiesta.Domain.Entities.Users
{
    public class UserFriend
    {
        public string UserId { get; private set; }
        public FiestaUser User { get; private set; }

        public string FriendId { get; private set; }
        public FiestaUser Friend { get; private set; }

        public bool IsFriendRequest { get; private set; }

        public UserFriend(FiestaUser user, FiestaUser friend)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            Friend = friend ?? throw new ArgumentNullException(nameof(friend));
            IsFriendRequest = true;
        }

        private UserFriend()
        {

        }

        public void ConfirmFriendRequest()
        {
            IsFriendRequest = false;
        }
    }
}
