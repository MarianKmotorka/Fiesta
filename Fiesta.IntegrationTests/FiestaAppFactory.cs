using System.Linq;
using Fiesta.Infrastracture.Persistence;
using Fiesta.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Fiesta.IntegrationTests
{
    public class FiestaAppFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<FiestaDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<FiestaDbContext>(options => { options.UseInMemoryDatabase(FiestaDbContext.TestDbName); });
            });
        }
    }
}
