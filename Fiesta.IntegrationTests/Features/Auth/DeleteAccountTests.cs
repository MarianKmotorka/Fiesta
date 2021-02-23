using System.Net.Http;
using System.Threading.Tasks;
using Fiesta.Application.Features.Auth;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Fiesta.IntegrationTests.Features.Auth
{
    [Collection(nameof(FiestaAppFactory))]
    public class DeleteAccountTests : WebAppTestBase
    {
        public DeleteAccountTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenValidPassword_WhenDeleteAccountWithPassword_AccountIsDeleted()
        {
            var registerRequest = new RegisterWithEmailAndPassword.Command
            {
                Email = "unique@email.com",
                FirstName = "Jozko",
                LastName = "Javorsky",
                Password = "Pass123"
            };

            var registerResponse = await NotAuthedClient.PostAsJsonAsync("api/auth/register", registerRequest);

            registerResponse.EnsureSuccessStatusCode();

            var authUser = await ArrangeDb.Users.SingleAsync(x => x.Email == registerRequest.Email);
            authUser.EmailConfirmed = true;
            await ArrangeDb.SaveChangesAsync();

            using var client = CreateClientForUser(authUser);

            var deleteResponse = await client.DeleteAsync($"/api/auth/delete-account-with-password?password={registerRequest.Password}");

            deleteResponse.EnsureSuccessStatusCode();

            var deletedUser = await AssertDb.Users.SingleOrDefaultAsync(x => x.Email == registerRequest.Email);
            deletedUser.Should().BeNull();

            var deletedFiestaUser = await AssertDb.FiestaUsers.SingleOrDefaultAsync(x => x.Email == registerRequest.Email);
            deletedFiestaUser.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task GivenInvalidPassword_WhenDeleteAccountWithPassword_BadRequestIsReturned()
        {

        }

        [Fact]
        public async Task GivenValidCode_WhenDeleteAccountWithGoogle_AccountIsDeleted()
        {

        }

        [Fact]
        public async Task GivenInvalidCode_WhenDeleteAccountWithGoogle_BadRequestIsReturned()
        {

        }
    }
}
