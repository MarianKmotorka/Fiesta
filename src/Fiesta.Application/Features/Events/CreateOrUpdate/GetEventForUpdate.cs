using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Features.Common;
using Fiesta.Application.Utils;
using Fiesta.Domain.Entities.Events;
using MediatR;

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
                var @event = await _db.Events.Select(x => new Response
                {
                    Id = x.Id,
                    Name = x.Name,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Description = x.Description,
                    BannerUrl = null,
                    Location = $"{x.Location.City}, {x.Location.State}",
                    GoogleMapsUrl = x.Location.GoogleMapsUrl,
                    AccessibilityType = x.AccessibilityType,
                    AttendeesCount = _db.EventAttendees.Count(x => x.EventId == request.Id),
                    Organizer = new UserDto
                    {
                        Id = x.Organizer.Id,
                        Username = x.Organizer.Username,
                        PictureUrl = x.Organizer.PictureUrl,
                    }
                })
                .SingleOrNotFoundAsync(x => x.Id == request.Id, cancellationToken);

                return @event;
            }
        }

        public class Response
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public string BannerUrl { get; set; }

            public string Description { get; set; }

            public DateTime StartDate { get; set; }

            public DateTime EndDate { get; set; }

            public AccessibilityType AccessibilityType { get; set; }

            public int AttendeesCount { get; set; }

            public string Location { get; set; }

            public string GoogleMapsUrl { get; set; }

            public UserDto Organizer { get; set; }
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
