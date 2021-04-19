using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Domain.Entities.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Fiesta.Application.Features.Selectors
{
    public class EventsAndUsersSelector
    {
        public class Query : IRequest<List<ResponseDto>>
        {
            public string Search { get; set; }

            public string CurrentUserId { get; set; }

            public FiestaRoleEnum Role { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<ResponseDto>>
        {
            private readonly IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<List<ResponseDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var usersQuery = _db.FiestaUsers.AsNoTracking()
                    .Select(x => new ResponseDto
                    {
                        Id = x.Id,
                        DisplayName = x.Username,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        PictureUrl = x.PictureUrl,
                        Description = null,
                        StartDate = null,
                        City = null,
                        State = null,
                        Type = ItemType.User,
                    });

                var eventsQuery = _db.Events.AsNoTracking()
                    .Where(x => request.Role == FiestaRoleEnum.Admin
                                || x.OrganizerId == request.CurrentUserId
                                || x.AccessibilityType == AccessibilityType.Public
                                || (x.AccessibilityType == AccessibilityType.FriendsOnly && x.Organizer.Friends.Any(f => f.FriendId == request.CurrentUserId))
                                || x.Attendees.Any(a => a.AttendeeId == request.CurrentUserId)
                                || x.Invitations.Any(i => i.InviteeId == request.CurrentUserId))
                    .Select(x => new ResponseDto
                    {
                        Id = x.Id,
                        DisplayName = x.Name,
                        FirstName = null,
                        LastName = null,
                        PictureUrl = x.BannerUrl,
                        Description = x.Description,
                        StartDate = x.StartDate,
                        City = x.Location.City,
                        State = x.Location.State,
                        Type = ItemType.Event,
                    });

                var query = usersQuery.Union(eventsQuery);

                if (!string.IsNullOrEmpty(request.Search))
                    query = query.Where(x => x.DisplayName.Contains(request.Search) || (x.FirstName + " " + x.LastName).Contains(request.Search));

                return await query.OrderBy(x => x.DisplayName).Take(25).ToListAsync(cancellationToken);
            }
        }

        public class ResponseDto
        {
            public string Id { get; set; }

            public string DisplayName { get; set; }

            public string FullName { get => FirstName is null ? null : $"{FirstName} {LastName}"; }

            public string Description { get; set; }

            public DateTime? StartDate { get; set; }

            public string PictureUrl { get; set; }

            public string Location { get => City is null ? null : $"{City}, {State}"; }

            public ItemType Type { get; set; }

            [JsonIgnore]
            public string FirstName { get; set; }

            [JsonIgnore]
            public string LastName { get; set; }

            [JsonIgnore]
            public string City { get; set; }

            [JsonIgnore]
            public string State { get; set; }
        }

        public enum ItemType
        {
            Event = 0,
            User = 1
        }
    }
}
