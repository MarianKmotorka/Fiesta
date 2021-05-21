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
    public class GetEventJoinRequests
    {
        public class Query : IRequest<QueryResponse<ResponseDto>>
        {
            [JsonIgnore]
            public string EventId { get; set; }

            public QueryDocument QueryDocument { get; set; } = new();

            [JsonIgnore]
            public string Search { get; set; }
        }

        public class Handler : IRequestHandler<Query, QueryResponse<ResponseDto>>
        {
            private IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<QueryResponse<ResponseDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _db.EventJoinRequests.AsNoTracking().Where(x => x.EventId == request.EventId);

                if (!string.IsNullOrEmpty(request.Search))
                    query = query
                        .Where(x => x.InterestedUser.Username.Contains(request.Search) || (x.InterestedUser.FirstName + " " + x.InterestedUser.LastName).Contains(request.Search));

                return await query
                    .OrderBy(x => x.InterestedUser.Username)
                    .Select(x => new ResponseDto
                    {
                        InterestedUser = new UserDto
                        {
                            Id = x.InterestedUserId,
                            Username = x.InterestedUser.Username,
                            PictureUrl = x.InterestedUser.PictureUrl,
                            FirstName = x.InterestedUser.FirstName,
                            LastName = x.InterestedUser.LastName,
                        },
                    })
                    .BuildResponse(request.QueryDocument, cancellationToken);
            }
        }

        public class ResponseDto
        {
            public UserDto InterestedUser { get; set; }
        }

        public class AuthorizationCheck : IAuthorizationCheck<Query>
        {
            public async Task<bool> IsAuthorized(Query request, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken)
                => await Helpers.IsOrganizerOrAdmin(request.EventId, db, currentUserService, cancellationToken);
        }
    }
}
