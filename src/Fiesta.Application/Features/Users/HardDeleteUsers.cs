using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Domain.Entities.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
            private readonly IImageService _imageService;
            private readonly ILogger<Handler> _logger;

            public Handler(IFiestaDbContext db, IImageService imageService, ILogger<Handler> logger)
            {
                _db = db;
                _imageService = imageService;
                _logger = logger;
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

                await DeleteUserImages(deletedUsers, cancellationToken);

                _db.FiestaUsers.RemoveRange(deletedUsers);
                await _db.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }

            private async Task DeleteUserImages(IEnumerable<FiestaUser> users, CancellationToken cancellationToken)
            {
                foreach (var user in users)
                {
                    if (string.IsNullOrEmpty(user.PictureUrl) || !user.PictureUrl.Contains(_imageService.Domain))
                        continue;

                    var path = CloudinaryPaths.ProfilePicture(user.Id);
                    var result = await _imageService.DeleteImageFromCloud(path, cancellationToken);

                    if (result.Failed)
                        _logger.LogWarning($"Image with path {path} failed to be deleted. Reason: {string.Join(',', result.Errors)}");
                }
            }
        }
    }
}
