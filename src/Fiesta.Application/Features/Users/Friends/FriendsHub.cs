using System.Threading.Tasks;
using Fiesta.Application.Common;
using Fiesta.Application.Common.Interfaces;

namespace Fiesta.Application.Features.Users.Friends
{
    public class FriendsHub : HubBase<IFriendsClient>
    {
        public FriendsHub(ICurrentUserService currentUser) : base(currentUser)
        {
        }
    }

    public interface IFriendsClient
    {
        Task ReceiveFriendRequest(FriendRequestDto friendRequest);
        Task RemoveFriendRequest(string friendId);
    }
}
