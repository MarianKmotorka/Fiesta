using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Models;
using Fiesta.Application.Features.Auth.CommonDtos;
using Fiesta.Infrastracture.Persistence;
using Fiesta.IntegrationTests.Assets;
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
        private IGoogleService _googleServiceMock = Substitute.For<IGoogleService>();

        public FiestaAppFactory()
        {
            _emailServiceMock
                .SendVerificationEmail(default, default, default)
                .ReturnsForAnyArgs(Task.FromResult(new FluentEmail.Core.Models.SendResponse()));

            _googleServiceMock
                .GetUserInfoModelForLogin(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(x => x.Args()[0].ToString() == "validCode"
                    ? Task.FromResult(Result.Success(GoogleAssets.JohnyUserInfoModel))
                    : Task.FromResult(Result<GoogleUserInfoModel>.Failure(ErrorCodes.InvalidCode)));
            _googleServiceMock
                .GetUserInfoModelForConnectAccount(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(x => x.Args()[0].ToString() == "validCode"
                    ? Task.FromResult(Result.Success(GoogleAssets.JohnyUserInfoModel))
                    : Task.FromResult(Result<GoogleUserInfoModel>.Failure(ErrorCodes.InvalidCode)));


        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.ConfigureServices(services =>
            {
                ReplaceDb(services);
                ReplaceEmailService(services);
                ReplaceGoogleService(services);
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

            services.AddTransient(_ => _emailServiceMock);
        }

        private void ReplaceDb(IServiceCollection services)
        {
            var dbContextDescriptor = services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<FiestaDbContext>));
            if (dbContextDescriptor is not null)
                services.Remove(dbContextDescriptor);

            services.AddDbContext<FiestaDbContext>(options => { options.UseInMemoryDatabase(FiestaDbContext.TestDbName); });
        }

        private void ReplaceGoogleService(IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(x => x.ServiceType == typeof(IGoogleService));
            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddScoped(_ => _googleServiceMock);
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
