using System.Linq;
using Fiesta.Infrastracture.Persistence;
using Fiesta.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestBase.Mocks;

namespace Fiesta.WebApi.Tests
{
    public class FiestaAppFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder
                .ConfigureServices(services =>
                {
                    ReplaceDb(services);
                    ReplaceService(services, IGoogleServiceMock.Mock);
                    ReplaceService(services, IEmailServiceMock.Mock);
                    ReplaceService(services, IImageServiceMock.Mock);
                })
                .ConfigureAppConfiguration(config =>
                {
                    config.Add(new TestConfigSource());
                })
                .UseEnvironment("Testing");
        }

        private void ReplaceDb(IServiceCollection services)
        {
            var dbContextDescriptor = services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<FiestaDbContext>));
            if (dbContextDescriptor is not null)
                services.Remove(dbContextDescriptor);

            services.AddDbContext<FiestaDbContext>(options => { options.UseInMemoryDatabase(FiestaDbContext.TestDbName); });
        }


        private void ReplaceService<T>(IServiceCollection services, T mockService) where T : class
        {
            var descriptor = services.SingleOrDefault(x => x.ServiceType == typeof(T));
            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddTransient(_ => mockService);
        }
    }

    public class TestConfigSource : IConfigurationSource
    {
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new TestConfigProvider();
        }
    }

    public class TestConfigProvider : ConfigurationProvider
    {
        public override void Load()
        {
            Set("IsTesting", "true");
        }
    }
}
