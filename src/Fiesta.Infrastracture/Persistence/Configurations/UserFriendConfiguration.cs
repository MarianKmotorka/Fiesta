using Fiesta.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fiesta.Infrastracture.Persistence.Configurations
{
    internal class UserFriendConfiguration : IEntityTypeConfiguration<UserFriend>
    {
        public void Configure(EntityTypeBuilder<UserFriend> builder)
        {
            builder.HasKey(uf => new { uf.UserId, uf.FriendId });
            builder.HasOne(uf => uf.Friend).WithMany().HasForeignKey(fk => fk.FriendId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(uf => uf.User).WithMany(f => f.Friends).HasForeignKey(fk => fk.UserId);
            builder.ToTable("UserFriends");
        }
    }
}
