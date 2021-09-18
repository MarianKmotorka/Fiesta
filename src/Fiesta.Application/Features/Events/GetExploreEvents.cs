﻿using System;
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

            public OnlineFilter OnlineFilter { get; set; }
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
                var query = _db.Events.AsNoTracking()
                    .Where(x => x.StartDate > DateTime.UtcNow)
                    .Where(x => x.Attendees.All(x => x.AttendeeId != request.CurrentUserId))
                    .Where(x => x.OrganizerId != request.CurrentUserId)
                    .Where(x => x.AccessibilityType == AccessibilityType.Public ||
                               (x.AccessibilityType == AccessibilityType.FriendsOnly && x.Organizer.Friends.Any(f => f.FriendId == request.CurrentUserId)));

                query = request.OnlineFilter switch
                {
                    OnlineFilter.OfflineOnly => query.Where(x => x.Location != null),
                    OnlineFilter.OnlineOnly => query.Where(x => x.ExternalLink != null),
                    _ => query
                };

                return await query
                    .Select(x => new ResponseDto
                    {
                        Id = x.Id,
                        AccessibilityType = x.AccessibilityType,
                        OrganizerUsername = x.Organizer.Username,
                        OrganizerId = x.OrganizerId,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        Name = x.Name,
                        Description = x.Description,
                        BannerUrl = x.BannerUrl,
                        OrganizerPictureUrl = x.Organizer.PictureUrl,
                        City = x.Location.City,
                        State = x.Location.State,
                        ExternalLink = x.ExternalLink,
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

            public string OrganizerUsername { get; set; }

            public string OrganizerId { get; set; }

            public string Description { get; set; }

            public int Capacity { get; set; }

            public int AttendeesCount { get; set; }
        }

        public enum OnlineFilter
        {
            All = 0,
            OnlineOnly = 1,
            OfflineOnly = 2
        }
    }
}
