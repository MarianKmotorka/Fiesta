using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Features.Common;
using Fiesta.Application.Features.Events.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Fiesta.Application.Features.Events
{
    public class UsersForEventInvitationSelector
    {
        public class Query : IRequest<List<ResponseDto>>
        {
            public string EventId { get; set; }

            public string Search { get; set; }

            public string CurrentUserId { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<ResponseDto>>
        {
            private readonly IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<List<ResponseDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var invitedUserIds = _db.EventInvitations.Where(x => x.EventId == request.EventId).Select(x => x.InviteeId);
                var attendeeIds = _db.EventAttendees.Where(x => x.EventId == request.EventId).Select(x => x.AttendeeId);
                var organizerId = _db.Events.Where(x => x.Id == request.EventId).Select(x => x.OrganizerId);
                var omitIds = invitedUserIds.Union(attendeeIds).Union(organizerId);

                var query = _db.FiestaUsers.Where(x => !omitIds.Contains(x.Id)).Select(x => new ResponseDto
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    PictureUrl = x.PictureUrl,
                    Username = x.Username,
                    IsFriend = _db.UserFriends.Any(uf => uf.FriendId == x.Id && uf.UserId == request.CurrentUserId)
                });

                if (!string.IsNullOrEmpty(request.Search))
                    query = query.Where(x => x.Username.Contains(request.Search) || (x.FirstName + " " + x.LastName).Contains(request.Search));

                return await query
                    .OrderByDescending(x => x.IsFriend)
                    .ThenBy(x => x.Username)
                    .Take(25)
                    .ToListAsync(cancellationToken);
            }
        }

        public class ResponseDto : UserDto
        {
            [JsonIgnore]
            public bool IsFriend { get; set; }
        }

        public class AuthorizationCheck : IAuthorizationCheck<Query>
        {
            public async Task<bool> IsAuthorized(Query request, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken)
                => await Helpers.IsOrganizerOrAdmin(request.EventId, db, currentUserService, cancellationToken);
        }
    }
}
