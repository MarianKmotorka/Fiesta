using Fiesta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fiesta.Infrastracture.Persistence.Configurations
{
    internal class FiestaUserConfiguration : IEntityTypeConfiguration<FiestaUser>
    {
        public void Configure(EntityTypeBuilder<FiestaUser> builder)
        {
            builder.Property(x => x.Id).HasMaxLength(36);
        }
    }
}
