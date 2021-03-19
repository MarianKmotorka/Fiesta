using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.IntegrationTests;
using Fiesta.WebApi.Middleware.ExceptionHanlding;
using FluentAssertions;
using TestBase.Assets;
using TestBase.Helpers;
using Xunit;

namespace Fiesta.WebApi.Tests.Features.Users
{
    [Collection(nameof(FiestaAppFactory))]
    public class UpdateUserTests : WebAppTestBase
    {
        public UpdateUserTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenUniqueUsername_WhenUpdatingUsername_UsernameIsChanged()
        {
            var result = await Client.PatchAsJsonAsync($"api/users/{LoggedInUserId}", new { username = "Majstre" });

            result.EnsureSuccessStatusCode();

            var fiestaUser = await AssertDb.FiestaUsers.FindAsync(LoggedInUserId);

            fiestaUser.Username.Should().Be("Majstre");

            var authUser = await AssertDb.Users.FindAsync(LoggedInUserId);

            authUser.UserName.Should().Be("Majstre");
        }

        [Fact]
        public async Task GivenNotUniqueUsername_WhenUpdatingUsername_BadRequestIsReturned()
        {
            //Note: Seeds basic user with Username=Bobby
            ArrangeDb.SeedBasicUser();
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PatchAsJsonAsync($"api/users/{LoggedInUserId}", new { username = "Bobby" });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var errorResposne = await response.Content.ReadAsAsync<ErrorResponse>();
            errorResposne.Should().BeEquivalentTo(new
            {
                ErrorCode = "ValidationError",
                ErrorDetails = new object[]
                {
                    new
                    {
                        Code = ErrorCodes.AlreadyExists,
                        PropertyName="username"
                    }
                }
            });
        }
    }
}