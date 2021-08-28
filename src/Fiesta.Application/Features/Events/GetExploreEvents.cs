using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Queries;
using Fiesta.Application.Features.Common;
using Fiesta.Domain.Entities.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Events
{
    /// <summary>
    /// Returns events where user is not attandee nor organizer
    /// </summary>
    public class GetExploreEvents
    {
        public class Query : IRequest<QueryResponse<ResponseDto>>
        {
            [JsonIgnore]
            public string CurrentUserId { get; set; }

            public QueryDocument QueryDocument { get; set; } = new();
        }

        public class Handler : IRequestHandler<Query, QueryResponse<ResponseDto>>
        {
            private readonly IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<QueryResponse<ResponseDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _db.Events.AsNoTracking()
                    .Where(x => x.Attendees.All(x => x.AttendeeId != request.CurrentUserId))
                    .Where(x => x.OrganizerId != request.CurrentUserId)
                    .Where(x => x.AccessibilityType == AccessibilityType.Public ||
                               (x.AccessibilityType == AccessibilityType.FriendsOnly && x.Attendees.Any(a => a.Attendee.Friends.Any(f => f.FriendId == request.CurrentUserId))))
                    .Select(x => new ResponseDto
                    {
                        Id = x.Id,
                        AccessibilityType = x.AccessibilityType,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        Name = x.Name,
                        Description = x.Description,
                        BannerUrl = x.BannerUrl,
                        OrganizerPictureUrl = x.Organizer.PictureUrl,
                        City = x.Location.City,
                        State = x.Location.State,
                        Capacity = x.Capacity,
                        AttendeesCount = x.Attendees.Count(),
                    })
                    .OrderBy(x => x.StartDate)
                    .BuildResponse(request.QueryDocument, cancellationToken);
            }
        }

        public class ResponseDto : EventDto
        {
            public string OrganizerPictureUrl { get; set; }

            public string Description { get; set; }

            public int Capacity { get; set; }

            public int AttendeesCount { get; set; }
        }
    }
}
