using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Fiesta.Application.Common.Constants;
using Fiesta.Domain.Entities.Users;
using Fiesta.Infrastracture.Auth;
using Fiesta.WebApi.Tests.Helpers;
using TestBase;

namespace Fiesta.WebApi.Tests
{
    public abstract class WebAppTestBase : DbTestBase, IDisposable
    {
        public HttpClient Client { get; }

        public HttpClient NotAuthedClient { get; }

        public FiestaAppFactory Factory { get; }

        public string LoggedInUserId => "9d86035e-3e83-42f4-a319-0a7235212e6a";

        public WebAppTestBase(FiestaAppFactory factory)
        {
            Factory = factory;
            Client = factory.CreateClient();
            NotAuthedClient = factory.CreateClient();

            Authenticate(Client);
        }

        public HttpClient CreateClientForUser(AuthUser user)
        {
            var client = Factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.GetAccessToken(Factory));
            return client;
        }

        private void Authenticate(HttpClient client)
        {
            var user = new AuthUser("basicUser@test.com", FiestaRoleEnum.BasicUser, AuthProviderEnum.EmailAndPassword, "BasicUser")
            {
                Id = LoggedInUserId,
                PasswordHash = "some_fake_passsword_hash",
            };

            var fiestaUser = FiestaUser.CreateWithId(user.Id, user.Email, user.UserName);

            ArrangeDb.AddRangeAsync(user, fiestaUser);
            ArrangeDb.SaveChanges();

            var accessToken = user.GetAccessToken(Factory);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public new void Dispose()
        {
            base.Dispose();
            Client.Dispose();
            NotAuthedClient.Dispose();
        }
    }
}
