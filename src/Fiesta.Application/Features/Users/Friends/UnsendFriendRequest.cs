using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Utils;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Fiesta.Application.Features.Users.Friends
{
    public class UnsendFriendRequest
    {
        public class Command : IRequest<Unit>
        {
            [JsonIgnore]
            public string UserId { get; set; }
            public string FriendId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly IFiestaDbContext _db;
            private readonly IHubContext<FriendsHub, IFriendsClient> _friendsHub;

            public Handler(IFiestaDbContext db, IHubContext<FriendsHub, IFriendsClient> friendsHub)
            {
                _db = db;
                _friendsHub = friendsHub;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var friendRequest = await _db.FriendRequests.SingleOrNotFoundAsync(x => x.FromId == request.UserId && x.ToId == request.FriendId, cancellationToken);
                _db.FriendRequests.Remove(friendRequest);

                if (FriendsHub.UserConnections.TryGetValue(friendRequest.ToId, out var connectionIds))
                    await _friendsHub.Clients.Clients(connectionIds).RemoveFriendRequest(friendRequest.FromId);

                await _db.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}
