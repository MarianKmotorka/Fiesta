using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Queries;
using Fiesta.Application.Features.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Events
{
    public class GetAllEvents
    {
        public class Query : IRequest<QueryResponse<EventDto>>
        {
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
                var query = _db.Events.AsNoTracking().IgnoreQueryFilters();

                if (!string.IsNullOrEmpty(request.Search))
                    query = query.Where(x => x.Name.Contains(request.Search));

                var users = await query.Select(x => new EventDto
                {
                    Id = x.Id,
                    AccessibilityType = x.AccessibilityType,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Name = x.Name,
                    BannerUrl = x.BannerUrl,
                    City = x.Location.City,
                    State = x.Location.State,
                    ExternalLink = x.ExternalLink,
                })
                .OrderBy(x => x.StartDate)
                .BuildResponse(request.QueryDocument, cancellationToken);

                return users;
            }
        }
    }
}
