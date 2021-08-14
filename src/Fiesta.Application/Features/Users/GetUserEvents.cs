﻿using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Queries;
using Fiesta.Application.Features.Common;
using Fiesta.Application.Utils;
using MediatR;

namespace Fiesta.Application.Features.Users
{
    public class GetUserEvents
    {
        public class Query : IRequest<QueryResponse<EventDto>>
        {
            [JsonIgnore]
            public string UserId { get; set; }

            public QueryDocument QueryDocument { get; set; } = new();
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
                var events = await _db.Events.Where(x => x.Attendees.Any(a => a.AttendeeId == request.UserId))
                    .Select(x => new EventDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        AccessibilityType = x.AccessibilityType,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        BannerUrl = "TODO: picture url"
                    }).BuildResponse(request.QueryDocument, cancellationToken);

                return events;
            }
        }

        public class AuthorizationCheck : IAuthorizationCheck<Query>
        {
            public Task<bool> IsAuthorized(Query request, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken)
                => Task.FromResult(currentUserService.IsResourceOwnerOrAdmin(request.UserId));
        }
    }
}
