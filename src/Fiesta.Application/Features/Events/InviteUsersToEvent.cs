using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Features.Events.Common;
using Fiesta.Application.Models.Notifications;
using Fiesta.Application.Utils;
using Fiesta.Domain.Entities.Events;
using Fiesta.Domain.Entities.Notifications;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Events
{
    public class InviteUsersToEvent
    {
        public class Command : IRequest
        {
            [JsonIgnore]
            public string CurrentUserId { get; set; }

            [JsonIgnore]
            public string EventId { get; set; }

            public List<string> InvitedIds { get; set; }
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
                var @event = await _db.Events.Include(x => x.Organizer).SingleOrNotFoundAsync(x => x.Id == request.EventId, cancellationToken);
                var invitedUsers = await _db.FiestaUsers.Where(x => request.InvitedIds.Contains(x.Id)).ToListAsync(cancellationToken);

                @event.AddInvitations(invitedUsers.ToArray());
                await SendNotification(@event.Invitations);

                await _db.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }

            private async Task SendNotification(IEnumerable<EventInvitation> invitations)
            {
                foreach (var invitation in invitations)
                {
                    var model = new EventInvitationCreatedNotification(invitation);
                    _db.Notifications.Add(new Notification(invitation.Invitee, model));

                    await Task.CompletedTask; // TODO replace with SignalR call
                }
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly IFiestaDbContext _db;

            public Validator(IFiestaDbContext db)
            {
                _db = db;

                RuleFor(x => x.InvitedIds)
                    .NotEmpty().WithErrorCode(ErrorCodes.Required)
                    .MustAsync(NotInvitedOrAttendee).WithErrorCode(ErrorCodes.AlreadyAttendeeOrInvited);
            }

            private async Task<bool> NotInvitedOrAttendee(Command command, List<string> invitedIds, CancellationToken cancellationToken)
            {
                var isOrganizer = invitedIds.Contains(command.CurrentUserId);
                if (isOrganizer)
                    return false;

                var alreadyInvited = await _db.Events.Where(x => x.Id == command.EventId)
                    .SelectMany(x => x.Invitations.Select(i => i.InviteeId))
                    .AnyAsync(x => invitedIds.Contains(x), cancellationToken);

                if (alreadyInvited)
                    return false;

                var alreadyAttendee = await _db.Events.Where(x => x.Id == command.EventId)
                    .SelectMany(x => x.Attendees.Select(a => a.AttendeeId))
                    .AnyAsync(x => invitedIds.Contains(x), cancellationToken);

                return !alreadyAttendee;
            }
        }

        public class AuthorizationCheck : IAuthorizationCheck<Command>
        {
            public async Task<bool> IsAuthorized(Command request, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken)
                 => await Helpers.IsOrganizerOrAdmin(request.EventId, db, currentUserService, cancellationToken);

        }
    }
}
