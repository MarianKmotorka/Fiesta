using Fiesta.Domain.Entities.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fiesta.Infrastracture.Persistence.Configurations
{
    public class EventJoinRequestConfiguration : IEntityTypeConfiguration<EventJoinRequest>
    {
        public void Configure(EntityTypeBuilder<EventJoinRequest> builder)
        {
            builder.ToTable("EventJoinRequests");
            builder.HasKey(x => new { x.EventId, x.InterestedUserId });
            builder.HasOne(x => x.InterestedUser).WithMany(x => x.SentEventJoinRequests).HasForeignKey(x => x.InterestedUserId);
            builder.HasOne(x => x.Event).WithMany(x => x.JoinRequests).HasForeignKey(x => x.EventId);

            builder.HasQueryFilter(x => !x.InterestedUser.IsDeleted);
        }
    }
}

