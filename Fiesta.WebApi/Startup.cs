using Autofac;
using Fiesta.Application;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Exceptions;
using Fiesta.Infrastracture.DependencyInjection;
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

            services.AddHttpContextAccessor();
            services.AddControllers();
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
                app.UseCors(config =>
                {
                    config.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            }

            app.UseHttpsRedirection();

            app.UseFiestaExceptionHandlingMiddleware();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
