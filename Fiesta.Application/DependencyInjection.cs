using System.Reflection;
using Fiesta.Application.Common.Behaviours;
using Fiesta.Application.Utils;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Fiesta.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            FluentValidationCamelCasePropertyNameResolver.UseFluentValidationCamelCasePropertyResolver();

            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
            services.AddHttpClient();
            return services;
        }
    }
}
