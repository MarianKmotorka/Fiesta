using System.Reflection;
using Fiesta.Application.BackgroundWorkers;
using Fiesta.Application.Common.Behaviours;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Options;
using Fiesta.Application.Utils;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fiesta.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            FluentValidationCamelCasePropertyNameResolver.UseFluentValidationCamelCasePropertyResolver();

            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationCheckBehavior<,>));
            services.AddHttpClient();

            services.AddHardDeleteUsersWorker(configuration);

            return services;
        }

        private static void AddHardDeleteUsersWorker(this IServiceCollection services, IConfiguration configuration)
        {
            if (!configuration.IsTesting())
                services.AddHostedService<HardDeleteUsersWorker>();

            var options = new HardDeleteUsersWorkerOptions();
            configuration.GetSection(nameof(HardDeleteUsersWorkerOptions)).Bind(options);
            services.AddSingleton(options);
        }
    }
}
