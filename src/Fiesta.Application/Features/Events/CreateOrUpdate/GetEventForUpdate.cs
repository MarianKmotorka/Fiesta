using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Features.Events.Common;
using Fiesta.Application.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Events.CreateOrUpdate
{
    public class GetEventForUpdate
    {
        public class Query : IRequest<Response>
        {
            public string Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Response>
        {
            private IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                var @event = await _db.Events.AsNoTracking().Select(x => new Response
                {
                    Id = x.Id,
                    Name = x.Name,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Description = x.Description,
                    Location = x.Location != null ? LocationDto.Map(x.Location) : null,
                    ExternalLink = x.ExternalLink,
                    AccessibilityType = x.AccessibilityType,
                    Capacity = x.Capacity,
                })
                .SingleOrNotFoundAsync(x => x.Id == request.Id, cancellationToken);

                return @event;
            }
        }

        public class Response : SharedDto
        {
        }

        public class AuthorizationCheck : IAuthorizationCheck<Query>
        {
            public async Task<bool> IsAuthorized(Query request, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken)
            {
                var resource = await db.Events.FindOrNotFoundAsync(cancellationToken, request.Id);
                return currentUserService.IsResourceOwnerOrAdmin(resource.OrganizerId);
            }
        }
    }
}
