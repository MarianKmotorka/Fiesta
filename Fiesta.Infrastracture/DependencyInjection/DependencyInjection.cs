using Fiesta.Application.Common.Interfaces;
using Fiesta.Infrastracture.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fiesta.Infrastracture.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FiestaDbContext>(options =>
              options.UseSqlServer(
                configuration.GetConnectionString("DbConnection"),
                builder => builder.MigrationsAssembly(typeof(FiestaDbContext).Assembly.FullName)));

            services.AddJwtAuthentication(configuration);
            services.AddScoped<IFiestaDbContext, FiestaDbContext>();

            return services;
        }
    }
}
