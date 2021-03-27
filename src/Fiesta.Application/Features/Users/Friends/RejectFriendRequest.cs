using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Utils;
using MediatR;
using Newtonsoft.Json;

namespace Fiesta.Application.Features.Users.Friends
{
    public class RejectFriendRequest
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

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var friendRequest = await _db.FriendRequests.SingleOrNotFoundAsync(x => x.FromId == request.FriendId && x.ToId == request.UserId, cancellationToken);
                _db.FriendRequests.Remove(friendRequest);

                await _db.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}
