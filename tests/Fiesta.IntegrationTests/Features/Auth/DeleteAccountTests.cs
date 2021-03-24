using System.Net.Http;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Features.Auth;
using Fiesta.WebApi.Middleware.ExceptionHanlding;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TestBase.Assets;
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

            var deletedFiestaUser = await AssertDb.FiestaUsers.IgnoreQueryFilters().SingleAsync(x => x.Email == registerRequest.Email);
            deletedFiestaUser.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task GivenInvalidPassword_WhenDeleteAccountWithPassword_BadRequestIsReturned()
        {
            var registerRequest = new RegisterWithEmailAndPassword.Command
            {
                Email = "unique@email.com",
                FirstName = "Jozko",
                LastName = "Javorsky",
                Password = "Pass123"
            };

            var registerResponse = await NotAuthedClient.PostAsJsonAsync("api/auth/register", registerRequest);

            var authUser = await ArrangeDb.Users.SingleAsync(x => x.Email == registerRequest.Email);
            authUser.EmailConfirmed = true;
            await ArrangeDb.SaveChangesAsync();

            using var client = CreateClientForUser(authUser);

            var deleteResponse = await client.DeleteAsync($"/api/auth/delete-account-with-password?password=BadPassword");

            var errorResposne = await deleteResponse.Content.ReadAsAsync<ErrorResponse>();
            errorResposne.Should().BeEquivalentTo(new
            {
                ErrorCode = "BadRequest",
                ErrorMessage = ErrorCodes.InvalidPassword
            });
        }

        [Fact]
        public async Task GivenValidCode_WhenDeleteAccountWithGoogle_AccountIsDeleted()
        {
            var response = await NotAuthedClient.PostAsJsonAsync("/api/auth/google-login", new { code = "validCode" });
            var user = await ArrangeDb.Users.SingleAsync(x => x.Email == GoogleAssets.JohnyUserInfoModel.Email);

            using var client = CreateClientForUser(user);

            var deleteResponse = await client.DeleteAsync($"/api/auth/delete-account-with-google?code=validCode");

            deleteResponse.EnsureSuccessStatusCode();

            var deletedUser = await AssertDb.Users.SingleOrDefaultAsync(x => x.Email == GoogleAssets.JohnyUserInfoModel.Email);
            deletedUser.Should().BeNull();

            var deletedFiestaUser = await AssertDb.FiestaUsers.IgnoreQueryFilters().SingleAsync(x => x.Email == GoogleAssets.JohnyUserInfoModel.Email);
            deletedFiestaUser.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task GivenInvalidCode_WhenDeleteAccountWithGoogle_BadRequestIsReturned()
        {
            var response = await NotAuthedClient.PostAsJsonAsync("/api/auth/google-login", new { code = "validCode" });
            var user = await ArrangeDb.Users.SingleAsync(x => x.Email == GoogleAssets.JohnyUserInfoModel.Email);

            using var client = CreateClientForUser(user);

            var deleteResponse = await client.DeleteAsync($"/api/auth/delete-account-with-google?code=invalidCode");

            var errorResposne = await deleteResponse.Content.ReadAsAsync<ErrorResponse>();
            errorResposne.Should().BeEquivalentTo(new
            {
                ErrorCode = "BadRequest",
                ErrorMessage = ErrorCodes.InvalidCode
            });
        }

        [Fact]
        public async Task GivenInvalidGoogleAccount_WhenDeleteAccountWithGoogle_BadRequestIsReturned()
        {
            var registerRequest = new RegisterWithEmailAndPassword.Command
            {
                Email = "unique@email.com",
                FirstName = "Jozko",
                LastName = "Javorsky",
                Password = "Pass123"
            };

            var registerResponse = await NotAuthedClient.PostAsJsonAsync("api/auth/register", registerRequest);

            var authUser = await ArrangeDb.Users.SingleAsync(x => x.Email == registerRequest.Email);
            authUser.EmailConfirmed = true;
            authUser.AddGoogleAuthProvider("random@email.com");
            await ArrangeDb.SaveChangesAsync();

            using var client = CreateClientForUser(authUser);

            var deleteResponse = await client.DeleteAsync($"/api/auth/delete-account-with-google?code=validCode");

            var errorResposne = await deleteResponse.Content.ReadAsAsync<ErrorResponse>();
            errorResposne.Should().BeEquivalentTo(new
            {
                ErrorCode = "BadRequest",
                ErrorMessage = ErrorCodes.GoogleAccountNotConnected
            });
        }
    }
}
