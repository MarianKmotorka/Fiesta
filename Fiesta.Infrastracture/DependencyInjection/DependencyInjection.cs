using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Options;
using Fiesta.Infrastracture.Auth;
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
                configuration.GetConnectionString("FiestaDb"),
                builder => builder.MigrationsAssembly(typeof(FiestaDbContext).Assembly.FullName)));

            services.AddJwtAuthentication(configuration);
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IFiestaDbContext, FiestaDbContext>();

            var cloudinaryOptions = new CloudinaryOptions();
            configuration.GetSection(nameof(CloudinaryOptions)).Bind(cloudinaryOptions);
            services.AddSingleton(cloudinaryOptions);

            return services;
        }
    }
}
