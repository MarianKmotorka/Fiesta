using Fiesta.Application.Common.Constants;
using Fiesta.Infrastracture.Auth;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TestBase.Assets;
using Xunit;

namespace Fiesta.IntegrationTests.Features.Auth
{
    [Collection(nameof(FiestaAppFactory))]
    public class ConnectGoogleAccountTests : WebAppTestBase
    {
        public ConnectGoogleAccountTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenEmailAndPasswordUser_WhenConnectingGoogleAccount_AccountIsConnected()
        {
            var response = await Client.PostAsJsonAsync("/api/auth/connect-google-account", new { Code = "validCode" });
            response.EnsureSuccessStatusCode();

            var user = await AssertDb.Users.SingleAsync(x => x.Id == LoggedInUserId);
            user.AuthProvider.Should().HaveFlag(AuthProviderEnum.Google);
            user.AuthProvider.Should().HaveFlag(AuthProviderEnum.EmailAndPassword);
            user.GoogleEmail.Should().Be(GoogleAssets.JohnyUserInfoModel.Email);
        }

        [Fact]
        public async Task GivenOtherUserWithGoogleEmail_WhenEmailAndPasswordUserConnectingSameGoogleAccount_BadRequestIsReturned()
        {
            var googleConnectedUser = new AuthUser("some@email.com", AuthProviderEnum.EmailAndPassword, "some");
            googleConnectedUser.AddGoogleAuthProvider(GoogleAssets.JohnyUserInfoModel.Email);
            ArrangeDb.Users.Add(googleConnectedUser);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync("/api/auth/connect-google-account", new { Code = "validCode" });
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var user = await AssertDb.Users.SingleAsync(x => x.Id == LoggedInUserId);
            user.AuthProvider.Should().Be(AuthProviderEnum.EmailAndPassword);
            user.GoogleEmail.Should().BeNull();
        }

        [Fact]
        public async Task GivenUserWithoutConfirmedEmail_WhenConnectingGoogleAccountWithSameEmail_EmailIsVerified()
        {
            var unverifiedEmailUser = new AuthUser(GoogleAssets.JohnyUserInfoModel.Email, AuthProviderEnum.EmailAndPassword, GoogleAssets.JohnyUserInfoModel.Name);
            ArrangeDb.Add(unverifiedEmailUser);
            await ArrangeDb.SaveChangesAsync();

            using var client = CreateClientForUser(unverifiedEmailUser);
            var response = await client.PostAsJsonAsync("/api/auth/connect-google-account", new { Code = "validCode" });
            response.EnsureSuccessStatusCode();

            var user = await AssertDb.Users.SingleAsync(x => x.Id == unverifiedEmailUser.Id);
            user.EmailConfirmed.Should().BeTrue();
        }

        [Fact]
        public async Task GivenUserWithGoogleAccount_WhenConnectingAnotherGoogleAccount_BadRequestIsReturned()
        {
            var user = new AuthUser("user@gmail.com", AuthProviderEnum.Google, "User");
            ArrangeDb.Add(user);
            await ArrangeDb.SaveChangesAsync();

            using var client = CreateClientForUser(user);
            var response = await client.PostAsJsonAsync("/api/auth/connect-google-account", new { Code = "validCode" });
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
