namespace Fiesta.Domain.Entities.Users
{
    public class UserFriends
    {
        public string UserId { get; set; }
        public FiestaUser User { get; set; }

        public string FriendId { get; set; }
        public FiestaUser Friend { get; set; }
    }
}
