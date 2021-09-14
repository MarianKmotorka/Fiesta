using Autofac;
using Fiesta.Application;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Exceptions;
using Fiesta.Application.Common.Models;
using Fiesta.Application.Features.Notifications;
using Fiesta.Application.Features.Users.Friends;
using Fiesta.Infrastracture.DependencyInjection;
using Fiesta.Infrastracture.Persistence;
using Fiesta.WebApi.Extensions;
using Fiesta.WebApi.Middleware.ExceptionHanlding;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fiesta.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwagger()
                    .AddFiestaAuthorization()
                    .AddApplication(Configuration)
                    .AddInfrastructure(Configuration);

            services.AddApplicationInsightsTelemetry();
            services.AddSignalR().AddNewtonsoftJsonProtocol();
            services.AddHealthChecks().AddDbContextCheck<FiestaDbContext>();

            services.AddHttpContextAccessor();
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = OptionalContractResolver.CreateReplacement(options.SerializerSettings.ContractResolver);
                });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(BadRequestException).Assembly)
                .AsClosedTypesOf(typeof(IAuthorizationCheck<>))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fiesta.WebApi v1"));
            }
            app.UseCors(config =>
            {
                config.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            });

            app.UseHttpsRedirection();

            app.UseFiestaExceptionHandlingMiddleware();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
                endpoints.MapHub<NotificationsHub>("/api/notifications-hub");
                endpoints.MapHub<FriendsHub>("/api/friends-hub");
            });
        }
    }
}
