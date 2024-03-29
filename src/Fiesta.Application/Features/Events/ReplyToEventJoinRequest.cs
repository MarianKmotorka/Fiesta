﻿using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Features.Common;
using Fiesta.Application.Features.Events.Common;
using Fiesta.Application.Features.Notifications;
using Fiesta.Application.Models.Notifications;
using Fiesta.Application.Utils;
using Fiesta.Domain.Entities.Events;
using Fiesta.Domain.Entities.Notifications;
using Fiesta.Domain.Entities.Users;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Events
{
    public class ReplyToEventJoinRequest
    {
        public class Command : IRequest
        {
            [JsonIgnore]
            public string EventId { get; set; }

            public string UserId { get; set; }

            public bool Accepted { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private IFiestaDbContext _db;
            private readonly IHubContext<NotificationsHub, INotificationsClient> _hub;
            private readonly ICurrentUserService _currentUserService;

            public Handler(IFiestaDbContext db, IHubContext<NotificationsHub, INotificationsClient> hub, ICurrentUserService currentUserService)
            {
                _db = db;
                _hub = hub;
                _currentUserService = currentUserService;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var joinRequest = await _db.EventJoinRequests
                    .SingleOrNotFoundAsync(x => x.EventId == request.EventId && x.InterestedUserId == request.UserId, cancellationToken);

                _db.EventJoinRequests.Remove(joinRequest);

                var invitation = await _db.EventInvitations
                    .SingleOrDefaultAsync(x => x.EventId == request.EventId && x.InviteeId == request.UserId, cancellationToken);

                if (invitation is not null)
                    _db.EventInvitations.Remove(invitation);

                var interestedUser = await _db.FiestaUsers.FindOrNotFoundAsync(cancellationToken, request.UserId);

                if (request.Accepted)
                {
                    var @event = await _db.Events.FindOrNotFoundAsync(cancellationToken, request.EventId);
                    @event.AddAttendee(interestedUser);
                }

                var currentUser = await _db.FiestaUsers.FindOrNotFoundAsync(cancellationToken, _currentUserService.UserId);

                await SendNotification(joinRequest, currentUser, interestedUser, request.Accepted, cancellationToken);

                await _db.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }

            private async Task SendNotification(EventJoinRequest joinRequest, FiestaUser responder, FiestaUser interestedUser, bool accepted, CancellationToken cancellationToken)
            {
                var notificationModel = new EventJoinRequestReplyNotification(joinRequest, responder, accepted);
                var notification = _db.Notifications.Add(new Notification(interestedUser, notificationModel)).Entity;
                await _db.SaveChangesAsync(cancellationToken);

                await _hub.Notify(notification);
            }
        }

        public class AuthorizationCheck : IAuthorizationCheck<Command>
        {
            public async Task<bool> IsAuthorized(Command request, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken)
                => await Helpers.IsOrganizerOrAdmin(request.EventId, db, currentUserService, cancellationToken);

        }
    }
}
