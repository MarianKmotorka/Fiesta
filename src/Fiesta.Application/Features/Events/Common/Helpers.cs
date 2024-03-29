﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Utils;
using Fiesta.Domain.Entities.Events;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Events.Common
{
    public static class Helpers
    {
        public static async Task<bool> CanViewEvent(string eventId, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken)
        {
            var @event = await db.Events.FindOrNotFoundAsync(cancellationToken, eventId);

            if (string.IsNullOrEmpty(currentUserService.UserId) && @event.AccessibilityType != AccessibilityType.Public)
                return false;

            if (@event.AccessibilityType == AccessibilityType.Public || currentUserService.IsResourceOwnerOrAdmin(@event.OrganizerId))
                return true;

            var isAttendeeOrInvited = await db.Events.Where(x => x.Id == eventId)
                .AnyAsync(x => x.Attendees.Any(a => a.AttendeeId == currentUserService.UserId)
                            || x.Invitations.Any(i => i.InviteeId == currentUserService.UserId), cancellationToken);

            if (@event.AccessibilityType == AccessibilityType.Private)
                return isAttendeeOrInvited;

            var isOrganizerFriend = await db.UserFriends
                .AnyAsync(x => x.UserId == currentUserService.UserId && x.FriendId == @event.OrganizerId, cancellationToken);

            if (@event.AccessibilityType == AccessibilityType.FriendsOnly)
                return isOrganizerFriend || isAttendeeOrInvited;

            throw new NotSupportedException($"Accessibility type {@event.AccessibilityType} not supported.");
        }

        public static async Task<bool> IsOrganizerOrAttendeeOrAdmin(string eventId, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken)
        {
            if (await IsOrganizerOrAdmin(eventId, db, currentUserService, cancellationToken))
                return true;

            return await db.Events
                .Where(x => x.Id == eventId)
                .AnyAsync(x => x.Attendees.Any(a => a.AttendeeId == currentUserService.UserId));
        }

        public static async Task<bool> IsOrganizerOrAdmin(string eventId, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(currentUserService.UserId))
                return false;

            var @event = await db.Events.FindOrNotFoundAsync(cancellationToken, eventId);
            return currentUserService.IsResourceOwnerOrAdmin(@event.OrganizerId);
        }
    }
}
