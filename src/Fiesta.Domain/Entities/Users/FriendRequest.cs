namespace Fiesta.Domain.Entities.Users
{
    public class FriendRequest
    {
        public string FromId { get; private set; }
        public FiestaUser From { get; private set; }

        public string ToId { get; private set; }
        public FiestaUser To { get; private set; }

        public FriendRequest(FiestaUser from, FiestaUser to)
        {
            From = from;
            To = to;
            FromId = from.Id;
            ToId = to.Id;
        }

        public FriendRequest()
        {

        }
    }
}
