using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Options;
using Fiesta.Infrastracture.Auth;
using Fiesta.IntegrationTests.Helpers;
using Fiesta.WebApi.Middleware.ExceptionHanlding;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Fiesta.IntegrationTests.Auth
{
    [Collection(nameof(FiestaAppFactory))]
    public class AddPasswordTests : WebAppTestBase
    {
        public AddPasswordTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenUserRegisteredWithGoogle_WhenAddingPassword_PasswordIsAdded()
        {
            using var client = Factory.CreateClient();
            var bearer = await SeedAndGetGoogleUserBearer();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);

            var addPassResponse = await client.PostAsJsonAsync("/api/auth/add-password", new { Password = "AddedPassword" });
            addPassResponse.EnsureSuccessStatusCode();

            var user = await AssertDb.Users.SingleAsync(x => x.Email == "google@user.com");
            user.AuthProvider.Should().HaveFlag(AuthProviderEnum.EmailAndPassword);
            user.AuthProvider.Should().HaveFlag(AuthProviderEnum.Google);
            user.PasswordHash.Should().NotBeNullOrEmpty();

        }

        [Fact]
        public async Task GivenUserRegisteredWithGoogle_WhenAddingInvalidPassword_BadRequestIsReturned()
        {
            using var client = Factory.CreateClient();
            var bearer = await SeedAndGetGoogleUserBearer();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);

            var addPassResponse = await client.PostAsJsonAsync("/api/auth/add-password", new { Password = "short" });
            addPassResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var errorResposne = await addPassResponse.Content.ReadAsAsync<ErrorResponse>();
            errorResposne.Should().BeEquivalentTo(new
            {
                ErrorCode = "ValidationError",
                ErrorDetails = new object[]
                {
                    new
                    {
                        Code = ErrorCodes.MinLength,
                        CustomState = JObject.FromObject(new { minLength = 6 }),
                        PropertyName="password"
                    }
                }
            });
        }

        [Fact]
        public async Task GivenUserWithEmailAndPassword_WhenAddingPassword_BadRequestIsReturned()
        {
            var addPassResponse = await Client.PostAsJsonAsync("/api/auth/add-password", new { Password = "AddedPassword" });
            addPassResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        private async Task<string> SeedAndGetGoogleUserBearer()
        {
            var user = new AuthUser("google@user.com", AuthProviderEnum.Google) { EmailConfirmed = true };
            ArrangeDb.Users.Add(user);
            await ArrangeDb.SaveChangesAsync();

            var options = Factory.Services.GetService(typeof(JwtOptions)) as JwtOptions;
            return user.GetAccessToken(options);
        }
    }
}
