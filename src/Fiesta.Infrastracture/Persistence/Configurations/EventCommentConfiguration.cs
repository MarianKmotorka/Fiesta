using Fiesta.Domain.Entities.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fiesta.Infrastracture.Persistence.Configurations
{
    public class EventCommentConfiguration : IEntityTypeConfiguration<EventComment>
    {
        public void Configure(EntityTypeBuilder<EventComment> builder)
        {
            builder.Property(x => x.Id).HasMaxLength(36);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.SenderId).IsRequired();
            builder.Property(x => x.EventId).IsRequired();
            builder.Property(x => x.Text).IsRequired();
            builder.HasOne(x => x.Parent).WithMany(x => x.Replies).OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(x => !x.Sender.IsDeleted);
        }
    }
}
