using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Domain.Common;
using Fiesta.Domain.Entities.Events;
using Fiesta.Domain.Entities.Users;
using Fiesta.Infrastracture.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Infrastracture.Persistence
{
    public class FiestaDbContext : IdentityDbContext<AuthUser, AuthRole, string>, IFiestaDbContext
    {
        public const string TestDbName = "TestDb";
        private readonly IMediator _mediator;

        public FiestaDbContext(DbContextOptions<FiestaDbContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }

        public DbSet<FiestaUser> FiestaUsers { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<FriendRequest> FriendRequests { get; set; }

        public DbSet<UserFriend> UserFriends { get; set; }

        public DbSet<EventInvitation> EventInvitations { get; set; }

        public DbSet<EventJoinRequest> EventJoinRequests { get; set; }

        public DbSet<EventAttendee> EventAttendees { get; set; }

        public DbSet<EventComment> EventComments { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            for (var i = 0; i < 3; i++) // To prevent infinite loop, number of iteration is capped to 3.
            {
                var events = ChangeTracker.Entries<BaseEntity>().SelectMany(po => po.Entity.ConsumeEvents()).ToList();
                if (events.Count == 0)
                    break;

                foreach (var @event in events)
                    await _mediator.Publish(@event, cancellationToken);
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.RemovePluralizingTableNameConvention();
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
