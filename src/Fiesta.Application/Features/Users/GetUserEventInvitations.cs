using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Queries;
using Fiesta.Application.Features.Common;
using Fiesta.Application.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Users
{
    public class GetUserEventInvitations
    {
        public class Query : IRequest<QueryResponse<EventDto>>
        {
            public string UserId { get; set; }

            public string Search { get; set; }

            public QueryDocument QueryDocument { get; set; } = new();

        }

        public class Handler : IRequestHandler<Query, QueryResponse<EventDto>>
        {
            private readonly IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<QueryResponse<EventDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var eventsQuery = _db.Events.AsNoTracking();

                if (!string.IsNullOrEmpty(request.Search))
                    eventsQuery = eventsQuery.Where(x => x.Name.Contains(request.Search));

                return await eventsQuery
                   .Where(x => x.Invitations.Any(i => i.InviteeId == request.UserId))
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
                   })
                   .OrderBy(x => x.StartDate)
                   .BuildResponse(request.QueryDocument, cancellationToken);
            }

            public class AuthorizationCheck : IAuthorizationCheck<Query>
            {
                public Task<bool> IsAuthorized(Query request, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken)
                    => Task.FromResult(currentUserService.IsResourceOwnerOrAdmin(request.UserId));
            }
        }
    }
}
