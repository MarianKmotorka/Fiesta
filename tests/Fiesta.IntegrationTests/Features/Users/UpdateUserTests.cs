using System.Threading.Tasks;
using Fiesta.IntegrationTests;
using FluentAssertions;
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

            var fiestaUser = await AssertDb.FiestaUsers.FindAsync(new[] { LoggedInUserId });

            fiestaUser.Username.Should().Be("Majstre");

            var authUser = await AssertDb.Users.FindAsync(new[] { LoggedInUserId });

            authUser.UserName.Should().Be("Majstre");
        }
    }
}