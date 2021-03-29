﻿using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Events
{
    public class DeleteEventAttendees
    {
        public class Command : IRequest
        {
            [JsonIgnore]
            public string EventId { get; set; }

            public List<string> RemoveUserIds { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var attendees = await _db.EventAttendees
                    .Where(x => x.EventId == request.EventId && request.RemoveUserIds.Contains(x.AttendeeId))
                    .ToListAsync(cancellationToken);

                foreach (var attendee in attendees)
                    _db.EventAttendees.Remove(attendee);

                await _db.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }

        public class AuthorizationCheck : IAuthorizationCheck<Command>
        {
            public async Task<bool> IsAuthorized(Command request, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken)
            {
                var @event = await db.Events.FindOrNotFoundAsync(cancellationToken, request.EventId);
                return @event.OrganizerId == currentUserService.UserId;
            }
        }
    }
}