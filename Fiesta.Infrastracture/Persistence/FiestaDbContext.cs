using System.Reflection;
using Fiesta.Infrastracture.Auth;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Infrastracture.Persistence
{
    internal class FiestaDbContext : IdentityDbContext<AuthUser, AuthRole, string>
    {
        public FiestaDbContext(DbContextOptions<FiestaDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.RemovePluralizingTableNameConvention();
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
