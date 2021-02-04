using Fiesta.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fiesta.Infrastracture.Persistence.Configurations
{
    internal class FiestaUserConfiguration : IEntityTypeConfiguration<FiestaUser>
    {
        public void Configure(EntityTypeBuilder<FiestaUser> builder)
        {
            builder.Property(x => x.Id).HasMaxLength(36);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
        }
    }
}
