using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Users
{
    public class HardDeleteUsers
    {
        public class Command : IRequest
        {
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var deletedUsers = await _db.FiestaUsers.IgnoreQueryFilters().Where(x => x.IsDeleted).ToListAsync(cancellationToken);

                foreach (var deletedUser in deletedUsers)
                {
                    var friendRelations = await _db.UserFriends.IgnoreQueryFilters().Where(x => x.UserId == deletedUser.Id || x.FriendId == deletedUser.Id).ToListAsync(cancellationToken);
                    _db.UserFriends.RemoveRange(friendRelations);

                    var friendRequests = await _db.FriendRequests.IgnoreQueryFilters().Where(x => x.ToId == deletedUser.Id || x.FromId == deletedUser.Id).ToListAsync(cancellationToken);
                    _db.FriendRequests.RemoveRange(friendRequests);

                    var organizedEvents = await _db.Events.IgnoreQueryFilters().Where(x => x.OrganizerId == deletedUser.Id).ToListAsync(cancellationToken);
                    _db.Events.RemoveRange(organizedEvents);

                    var eventInvitations = await _db.EventInvitations.IgnoreQueryFilters().Where(x => x.InviteeId == deletedUser.Id || x.InviterId == deletedUser.Id).ToListAsync(cancellationToken);
                    _db.EventInvitations.RemoveRange(eventInvitations);
                }

                _db.FiestaUsers.RemoveRange(deletedUsers);
                await _db.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}
