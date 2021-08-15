using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Queries;
using Fiesta.Application.Features.Common;
using Fiesta.Domain.Entities.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Users
{
    public class GetUserAttendedEvents
    {
        public class Query : IRequest<QueryResponse<EventDto>>
        {
            public string UserId { get; set; }

            public QueryDocument QueryDocument { get; set; } = new();

            public string Search { get; set; }

            public string CurrentUserId { get; set; }

            public FiestaRoleEnum Role { get; set; }
        }

        public class Handler : IRequestHandler<Query, QueryResponse<EventDto>>
        {
            private IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<QueryResponse<EventDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var eventsQuery = _db.Events.AsNoTracking();

                if (!string.IsNullOrEmpty(request.Search))
                    eventsQuery = eventsQuery.Where(x => x.Name.Contains(request.Search));

                var events = await eventsQuery
                    .Where(x => x.Attendees.Any(a => a.AttendeeId == request.UserId))
                    .Where(x => request.Role == FiestaRoleEnum.Admin
                                || x.OrganizerId == request.CurrentUserId
                                || x.AccessibilityType == AccessibilityType.Public
                                || (x.AccessibilityType == AccessibilityType.FriendsOnly && x.Organizer.Friends.Any(f => f.FriendId == request.CurrentUserId))
                                || x.Attendees.Any(a => a.AttendeeId == request.CurrentUserId)
                                || x.Invitations.Any(i => i.InviteeId == request.CurrentUserId))
                    .Select(x => new EventDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        AccessibilityType = x.AccessibilityType,
                        EndDate = x.EndDate,
                        StartDate = x.StartDate,
                        BannerUrl = x.BannerUrl,
                        City = x.Location.City,
                        State = x.Location.State,
                        IsCurrentUserAttending = x.Attendees.Any(x => x.AttendeeId == request.CurrentUserId) || x.OrganizerId == request.CurrentUserId
                    })
                    .OrderBy(x => x.StartDate)
                    .BuildResponse(request.QueryDocument, cancellationToken);

                return events;
            }
        }
    }
}
