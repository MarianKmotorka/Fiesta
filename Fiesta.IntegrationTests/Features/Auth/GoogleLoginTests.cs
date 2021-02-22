using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Features.Auth.CommonDtos;
using Fiesta.Infrastracture.Auth;
using Fiesta.IntegrationTests.Assets;
using Fiesta.WebApi.Middleware.ExceptionHanlding;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Fiesta.IntegrationTests.Features.Auth
{
    [Collection(nameof(FiestaAppFactory))]
    public class GoogleLoginTests : WebAppTestBase
    {
        public GoogleLoginTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenValidCode_WhenUserDoesNotExist_UserIsCreated()
        {
            // Note: GoogleService mock is configured to return success when code == "validCode"
            var response = await NotAuthedClient.GetAsync("/api/auth/google-code-callback?code=validCode");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsAsync<AuthResponse>();
            content.AccessToken.Should().NotBeNullOrEmpty();

            var user = await AssertDb.Users.SingleAsync(x => x.Email == GoogleAssets.JohnyUserInfoModel.Email);
            user.AuthProvider.Should().Be(AuthProviderEnum.Google);

            var fiestaUser = await AssertDb.FiestaUsers.SingleAsync(x => x.Email == GoogleAssets.JohnyUserInfoModel.Email);
            fiestaUser.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(3));
        }

        [Fact]
        public async Task GivenInValidCode_WhenLoggingIn_BadRequestIsReturned()
        {
            var response = await NotAuthedClient.GetAsync("/api/auth/google-code-callback?code=INVALID");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsAsync<ErrorResponse>();
            content.Should().BeEquivalentTo(new
            {
                ErrorCode = "BadRequest",
                ErrorMessage = ErrorCodes.InvalidCode
            });
        }

        [Fact]
        public async Task GivenEmailAndPasswordUser_WhenLoggingInWithSameEmailGoogleAccount_GoogleAccountIsConnected()
        {
            var emailAndPasswordUser = new AuthUser(GoogleAssets.JohnyUserInfoModel.Email, AuthProviderEnum.EmailAndPassword);
            ArrangeDb.Add(emailAndPasswordUser);
            await ArrangeDb.SaveChangesAsync();

            using var client = CreateClientForUser(emailAndPasswordUser);
            var response = await client.GetAsync("/api/auth/google-code-callback?code=validCode");
            response.EnsureSuccessStatusCode();

            var user = await AssertDb.Users.SingleAsync(x => x.Email == GoogleAssets.JohnyUserInfoModel.Email);
            user.AuthProvider.Should().HaveFlag(AuthProviderEnum.Google);
            user.AuthProvider.Should().HaveFlag(AuthProviderEnum.EmailAndPassword);
            user.GoogleEmail.Should().Be(GoogleAssets.JohnyUserInfoModel.Email);
        }

        [Fact]
        public async Task GivenUserWithEmailAndPasswordAndDifferentEmailGoogleAccount_WhenLoggingInWithSameEmailGoogleAccountAsEmailAndPassword_BadRequestIsReturned()
        {
            var user = new AuthUser(GoogleAssets.JohnyUserInfoModel.Email, AuthProviderEnum.EmailAndPassword);
            user.AddGoogleAuthProvider("some@gmail.com");
            ArrangeDb.Add(user);
            await ArrangeDb.SaveChangesAsync();

            using var client = CreateClientForUser(user);
            var response = await client.GetAsync("/api/auth/google-code-callback?code=validCode");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsAsync<ErrorResponse>();
            content.Should().BeEquivalentTo(new
            {
                ErrorCode = "BadRequest",
                ErrorMessage = ErrorCodes.AccountAlreadyConnectedToGoogleWithDifferentEmail
            });
        }
    }
}
