using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Features.Common;
using Fiesta.Application.Features.Events.Common;
using Fiesta.Application.Utils;
using Fiesta.Domain.Entities.Events;
using MediatR;

namespace Fiesta.Application.Features.Events
{
    public class GetEventDetail
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
                    BannerUrl = x.BannerUrl,
                    Location = $"{x.Location.City}, {x.Location.State}",
                    GoogleMapsUrl = x.Location.GoogleMapsUrl,
                    AccessibilityType = x.AccessibilityType,
                    Capacity = x.Capacity,
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

            public int Capacity { get; set; }

            public UserDto Organizer { get; set; }
        }

        public class AuthorizationCheck : IAuthorizationCheck<Query>
        {
            public async Task<bool> IsAuthorized(Query request, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken)
                => await Helpers.CanViewEvent(request.Id, db, currentUserService, cancellationToken);
        }
    }
}
