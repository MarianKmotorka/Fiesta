using System.Linq;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Infrastracture.Persistence;
using Fiesta.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Fiesta.IntegrationTests
{
    public class FiestaAppFactory : WebApplicationFactory<Startup>
    {
        private IEmailService _emailServiceMock = Substitute.For<IEmailService>();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.ConfigureServices(services =>
            {
                ReplaceDb(services);
                ReplaceEmailService(services);
            })
                .ConfigureAppConfiguration(config =>
                {
                    config.Add(new TestConfigSource());
                });
        }

        private void ReplaceEmailService(IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(x => x.ServiceType == typeof(IEmailService));
            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddTransient(x => _emailServiceMock);
        }

        private void ReplaceDb(IServiceCollection services)
        {
            var dbContextDescriptor = services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<FiestaDbContext>));
            if (dbContextDescriptor is not null)
                services.Remove(dbContextDescriptor);

            services.AddDbContext<FiestaDbContext>(options => { options.UseInMemoryDatabase(FiestaDbContext.TestDbName); });
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
