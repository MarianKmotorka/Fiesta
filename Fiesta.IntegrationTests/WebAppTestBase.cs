using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Fiesta.Application.Auth;
using Fiesta.Application.Auth.CommonDtos;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Infrastracture.Persistence;
using Fiesta.WebApi;
using Fiesta.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Fiesta.IntegrationTests
{
    public abstract class WebAppTestBase
    {
        public HttpClient Client { get; }
        public WebApplicationFactory<Startup> Factory { get; }
        public FiestaDbContext ArrangeDb { get; }
        public FiestaDbContext ActDb { get; }
        public FiestaDbContext AssertDb { get; }

        public WebAppTestBase()
        {
            Factory = new WebApplicationFactory<Startup>()
               .WithWebHostBuilder(builder =>
               {
                   builder.ConfigureServices(services =>
                   {
                       var descriptor = services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<FiestaDbContext>));

                       if (descriptor != null)
                           services.Remove(descriptor);

                       services.AddDbContext<FiestaDbContext>(options => { options.UseInMemoryDatabase(FiestaDbContext.TestDbName); });
                   });
               });


            Client = Factory.CreateClient();

            ActDb = CreateDb();
            ArrangeDb = CreateDb();
            AssertDb = CreateDb();

            Authenticate();
        }

        private static FiestaDbContext CreateDb()
        {
            var optionsBuilder = new DbContextOptionsBuilder<FiestaDbContext>().UseInMemoryDatabase(FiestaDbContext.TestDbName);
            return new FiestaDbContext(optionsBuilder.Options, new CurrentUserServiceStub());
        }

        private void Authenticate()
        {
            var command = new RegisterWithEmailAndPassword.Command
            {
                Email = "admin@fiesta.com",
                FirstName = "Admin",
                LastName = "Fiesta",
                Password = "Password123"
            };

            var response = Client.PostAsJsonAsync("api/auth/register", command).Result;
            response.EnsureSuccessStatusCode();

            var authUser = ArrangeDb.Users.Single(x => x.Email == command.Email);
            authUser.EmailConfirmed = true;
            ArrangeDb.SaveChanges();

            response = Client.PostAsJsonAsync("api/auth/login", new EmailPasswordRequest { Email = command.Email, Password = command.Password }).Result;
            response.EnsureSuccessStatusCode();
            var accessToken = response.Content.ReadAsAsync<AuthResponse>().Result.AccessToken;
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }

    public class CurrentUserServiceStub : ICurrentUserService
    {
        public string UserId => Guid.Empty.ToString();
    }

    //[CollectionDefinition(WebAppFactory.WebAppFactoryCollection)]
    //public class WebAppCollection : ICollectionFixture<WebAppFactory>
    //{
    //    // This class has no code, and is never created. Its purpose is simply
    //    // to be the place to apply [CollectionDefinition] and all the
    //    // ICollectionFixture<> interfaces.
    //}
}
