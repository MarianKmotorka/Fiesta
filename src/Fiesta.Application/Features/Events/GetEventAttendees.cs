using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Queries;
using Fiesta.Application.Utils;
using Fiesta.Domain.Entities.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Events
{
    public class GetEventAttendees
    {
        public class Query : IRequest<QueryResponse<AttendeeDto>>
        {
            [JsonIgnore]
            public string EventId { get; set; }

            public QueryDocument QueryDocument { get; set; } = new();
        }

        public class Handler : IRequestHandler<Query, QueryResponse<AttendeeDto>>
        {
            private IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<QueryResponse<AttendeeDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var @event = await _db.Events.Include(x => x.Organizer).SingleOrNotFoundAsync(x => x.Id == request.EventId, cancellationToken);

                return await _db.EventAttendees.Where(x => x.EventId == request.EventId)
                    .Select(x => new AttendeeDto
                    {
                        Id = x.AttendeeId,
                        PictureUrl = x.Attendee.PictureUrl,
                        Username = x.Attendee.Username
                    })
                    .BuildResponse(request.QueryDocument, cancellationToken);
            }
        }

        public class AuthorizationCheck : IAuthorizationCheck<Query>
        {
            public async Task<bool> IsAuthorized(Query request, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken)
            {
                var @event = await db.Events.FindOrNotFoundAsync(cancellationToken, request.EventId);

                if (@event.AccessibilityType == AccessibilityType.Public || @event.OrganizerId == currentUserService.UserId)
                    return true;

                var isAttendee = await db.EventAttendees
                    .Where(x => x.EventId == request.EventId)
                    .AnyAsync(x => x.AttendeeId == currentUserService.UserId, cancellationToken);

                if (@event.AccessibilityType == AccessibilityType.Private)
                    return isAttendee;

                var isOrganizerFriend = await db.UserFriends
                    .AnyAsync(x => x.UserId == currentUserService.UserId && x.FriendId == @event.OrganizerId, cancellationToken);

                if (@event.AccessibilityType == AccessibilityType.FriendsOnly)
                    return isOrganizerFriend || isAttendee;

                throw new NotSupportedException($"Accessibility type {@event.AccessibilityType} not supported.");
            }
        }

        public class AttendeeDto
        {
            public string Id { get; set; }

            public string Username { get; set; }

            public string PictureUrl { get; set; }
        }
    }
}
