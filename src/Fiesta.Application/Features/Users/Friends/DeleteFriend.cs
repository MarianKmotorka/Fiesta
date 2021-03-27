using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Fiesta.Application.Features.Users.Friends
{
    public class DeleteFriend
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
                var fiestaUser = await _db.FiestaUsers.Include(x => x.Friends).SingleOrNotFoundAsync(x => x.Id == request.UserId, cancellationToken);
                var fiestaFriend = await _db.FiestaUsers.Include(x => x.Friends).SingleOrNotFoundAsync(x => x.Id == request.FriendId, cancellationToken);

                fiestaUser.RemoveFriend(fiestaFriend);
                await _db.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}
