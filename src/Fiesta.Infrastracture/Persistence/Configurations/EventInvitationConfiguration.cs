using Fiesta.Domain.Entities.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fiesta.Infrastracture.Persistence.Configurations
{
    public class EventInvitationConfiguration : IEntityTypeConfiguration<EventInvitation>
    {
        public void Configure(EntityTypeBuilder<EventInvitation> builder)
        {
            builder.ToTable("EventInvitations");
            builder.HasKey(x => new { x.EventId, x.InviterId, x.InviteeId });
            builder.HasOne(x => x.Invitee).WithMany(x => x.RecievedEventInvitations).HasForeignKey(x => x.InviteeId);
            builder.HasOne(x => x.Inviter).WithMany(x => x.SentEventInvitations).HasForeignKey(x => x.InviterId);
            builder.HasOne(x => x.Event).WithMany(x => x.Invitations).HasForeignKey(x => x.EventId);

            builder.HasQueryFilter(x => !x.Invitee.IsDeleted && !x.Inviter.IsDeleted);
        }
    }
}

