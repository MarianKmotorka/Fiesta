using Fiesta.Domain.Entities.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fiesta.Infrastracture.Persistence.Configurations
{
    public class EventAttendeeConfiguration : IEntityTypeConfiguration<EventAttendee>
    {
        public void Configure(EntityTypeBuilder<EventAttendee> builder)
        {
            builder.ToTable("EventAttendees");
            builder.HasKey(x => new { x.EventId, x.AttendeeId });
            builder.HasOne(x => x.Attendee).WithMany(x => x.AttendedEvents).HasForeignKey(x => x.AttendeeId);
            builder.HasOne(x => x.Event).WithMany(x => x.Attendees).HasForeignKey(x => x.EventId);

            builder.HasQueryFilter(x => !x.Attendee.IsDeleted);
        }
    }
}
