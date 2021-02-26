using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Fiesta.Application.Common.Constants;
using Fiesta.Infrastracture.Auth;
using Fiesta.Infrastracture.Persistence;
using Fiesta.IntegrationTests.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace Fiesta.IntegrationTests
{
    public abstract class WebAppTestBase : IDisposable
    {
        public HttpClient Client { get; }

        public HttpClient NotAuthedClient { get; }

        public FiestaAppFactory Factory { get; }

        public FiestaDbContext ArrangeDb { get; }

        public FiestaDbContext ActDb { get; }

        public FiestaDbContext AssertDb { get; }

        public string LoggedInUserId => "9d86035e-3e83-42f4-a319-0a7235212e6a";

        public WebAppTestBase(FiestaAppFactory factory)
        {
            Factory = factory;
            Client = factory.CreateClient();
            NotAuthedClient = factory.CreateClient();

            ActDb = CreateDb();
            ArrangeDb = CreateDb();
            AssertDb = CreateDb();

            Authenticate(Client);
        }

        public HttpClient CreateClientForUser(AuthUser user)
        {
            var client = Factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.GetAccessToken(Factory));
            return client;
        }

        private static FiestaDbContext CreateDb()
        {
            var optionsBuilder = new DbContextOptionsBuilder<FiestaDbContext>().UseInMemoryDatabase(FiestaDbContext.TestDbName);
            return new FiestaDbContext(optionsBuilder.Options, Substitute.For<IMediator>());
        }

        private void Authenticate(HttpClient client)
        {
            var user = new AuthUser("admin@test.com", FiestaRoleEnum.Admin, AuthProviderEnum.EmailAndPassword)
            {
                Id = LoggedInUserId,
                PasswordHash = "some_fake_passsword_hash",
            };

            ArrangeDb.Users.Add(user);
            ArrangeDb.SaveChanges();

            var accessToken = user.GetAccessToken(Factory);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public void Dispose()
        {
            ArrangeDb.Database.EnsureDeleted();
            ArrangeDb.Dispose();
            ActDb.Dispose();
            AssertDb.Dispose();

            Client.Dispose();
            NotAuthedClient.Dispose();
        }
    }

    [CollectionDefinition(nameof(FiestaAppFactory))]
    public class WebAppCollection : ICollectionFixture<FiestaAppFactory>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
