using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Queries;
using Fiesta.Application.Features.Common;
using Fiesta.Application.Features.Events.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Events
{
    public class GetEventAttendees
    {
        public class Query : IRequest<QueryResponse<UserDto>>
        {
            [JsonIgnore]
            public string EventId { get; set; }

            public QueryDocument QueryDocument { get; set; } = new();

            [JsonIgnore]
            public string Search { get; set; }
        }

        public class Handler : IRequestHandler<Query, QueryResponse<UserDto>>
        {
            private IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<QueryResponse<UserDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _db.EventAttendees.AsNoTracking().Where(x => x.EventId == request.EventId)
                    .Select(x => new UserDto
                    {
                        Id = x.AttendeeId,
                        PictureUrl = x.Attendee.PictureUrl,
                        Username = x.Attendee.Username,
                        FirstName = x.Attendee.FirstName,
                        LastName = x.Attendee.LastName,
                    });

                if (!string.IsNullOrEmpty(request.Search))
                    query = query.Where(x => x.Username.Contains(request.Search) || (x.FirstName + " " + x.LastName).Contains(request.Search));

                return await query.BuildResponse(request.QueryDocument, cancellationToken);
            }
        }

        public class AuthorizationCheck : IAuthorizationCheck<Query>
        {
            public async Task<bool> IsAuthorized(Query request, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken)
                => await Helpers.CanViewEvent(request.EventId, db, currentUserService, cancellationToken);
        }
    }
}
