using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Infrastracture.Auth;
using Fiesta.WebApi.Middleware.ExceptionHanlding;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Fiesta.IntegrationTests.Features.Auth
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
            var user = new AuthUser("google@user.com", AuthProviderEnum.Google) { EmailConfirmed = true };
            ArrangeDb.Users.Add(user);
            await ArrangeDb.SaveChangesAsync();

            using var client = CreateClientForUser(user);
            var addPassResponse = await client.PostAsJsonAsync("/api/auth/add-password", new { Password = "AddedPassword" });
            addPassResponse.EnsureSuccessStatusCode();

            var userDb = await AssertDb.Users.SingleAsync(x => x.Email == "google@user.com");
            userDb.AuthProvider.Should().HaveFlag(AuthProviderEnum.EmailAndPassword);
            userDb.AuthProvider.Should().HaveFlag(AuthProviderEnum.Google);
            userDb.PasswordHash.Should().NotBeNullOrEmpty();

        }

        [Fact]
        public async Task GivenUserRegisteredWithGoogle_WhenAddingInvalidPassword_BadRequestIsReturned()
        {
            var user = new AuthUser("google@user.com", AuthProviderEnum.Google) { EmailConfirmed = true };
            ArrangeDb.Users.Add(user);
            await ArrangeDb.SaveChangesAsync();

            using var client = CreateClientForUser(user);
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
    }
}
