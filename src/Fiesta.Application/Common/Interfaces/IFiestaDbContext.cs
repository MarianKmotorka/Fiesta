using System;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Domain.Entities.Events;
using Fiesta.Domain.Entities.Notifications;
using Fiesta.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Common.Interfaces
{
    public interface IFiestaDbContext : IDisposable
    {
        DbSet<FiestaUser> FiestaUsers { get; }

        DbSet<Event> Events { get; }

        DbSet<FriendRequest> FriendRequests { get; }

        DbSet<UserFriend> UserFriends { get; }

        DbSet<EventInvitation> EventInvitations { get; }

        DbSet<EventJoinRequest> EventJoinRequests { get; }

        DbSet<EventAttendee> EventAttendees { get; }

        DbSet<EventComment> EventComments { get; }

        DbSet<Notification> Notifications { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
