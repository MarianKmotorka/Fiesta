﻿using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Infrastracture.Auth;
using Fiesta.IntegrationTests.Assets;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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
            var googleConnectedUser = new AuthUser("some@email.com", AuthProviderEnum.EmailAndPassword);
            googleConnectedUser.AddGoogleAuthProvider(GoogleAssets.JohnyUserInfoModel.Email);
            ArrangeDb.Users.Add(googleConnectedUser);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync("/api/auth/connect-google-account", new { Code = "validCode" });
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var user = await AssertDb.Users.SingleAsync(x => x.Id == LoggedInUserId);
            user.AuthProvider.Should().Be(AuthProviderEnum.EmailAndPassword);
            user.GoogleEmail.Should().BeNull();
        }
    }
}
