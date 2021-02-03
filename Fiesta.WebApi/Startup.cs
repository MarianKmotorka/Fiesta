using Fiesta.Application;
using Fiesta.Infrastracture.DependencyInjection;
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
            services.AddHttpContextAccessor();
            services.AddInfrastructure(Configuration);
            services.AddApplication();

            services.AddControllers();
            services.AddSwagger();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fiesta.WebApi v1"));
                //app.UseCors(config =>
                //{
                //    config.AllowAnyOrigin()
                //    .AllowAnyMethod()
                //    .AllowAnyHeader();
                //});
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
