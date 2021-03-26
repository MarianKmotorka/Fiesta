using Fiesta.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fiesta.Infrastracture.Persistence.Configurations
{
    public class FriendRequestConfiguration : IEntityTypeConfiguration<FriendRequest>
    {
        public void Configure(EntityTypeBuilder<FriendRequest> builder)
        {
            builder.HasKey(fr => new { fr.FromId, fr.ToId });
            builder.HasOne(fr => fr.From).WithMany(f => f.SentFriendRequests).HasForeignKey(fk => fk.FromId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(fr => fr.To).WithMany(f => f.RecievedFriendRequests).HasForeignKey(fk => fk.ToId);
            builder.ToTable("FriendRequests");
        }
    }
}
