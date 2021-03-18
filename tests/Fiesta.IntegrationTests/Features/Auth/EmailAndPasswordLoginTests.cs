using Fiesta.Application.Common.Constants;
using Fiesta.Application.Features.Auth;
using Fiesta.IntegrationTests;
using Fiesta.WebApi.Middleware.ExceptionHanlding;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Fiesta.WebApi.Tests.Features.Auth
{
    [Collection(nameof(FiestaAppFactory))]
    public class EmailAndPasswordLoginTests : WebAppTestBase
    {
        public EmailAndPasswordLoginTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenValidRequest_WhenLoggingInWithEmailAndPassword_UserIsLoggedIn()
        {
            var request = new RegisterWithEmailAndPassword.Command
            {
                Email = "unique@email.com",
                FirstName = "Jozko",
                LastName = "Javorsky",
                Password = "Pass123###"
            };

            await NotAuthedClient.PostAsJsonAsync("api/auth/register", request);

            var authUser = await AssertDb.Users.SingleAsync(x => x.Email == request.Email);
            authUser.EmailConfirmed = true;
            await AssertDb.SaveChangesAsync();

            var response = await NotAuthedClient.PostAsJsonAsync("/api/auth/login", new { emailOrUsername = authUser.Email, password = request.Password });
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GivenValidRequest_WhenLoggingInWithUsernameAndPassword_UserIsLoggedIn()
        {
            var request = new RegisterWithEmailAndPassword.Command
            {
                Email = "unique@email.com",
                FirstName = "Jozko",
                LastName = "Javorsky",
                Password = "Pass123###"
            };

            await NotAuthedClient.PostAsJsonAsync("api/auth/register", request);

            var authUser = await AssertDb.Users.SingleAsync(x => x.Email == request.Email);
            authUser.EmailConfirmed = true;
            await AssertDb.SaveChangesAsync();

            var response = await NotAuthedClient.PostAsJsonAsync("/api/auth/login", new { emailOrUsername = authUser.UserName, password = request.Password });
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GivenInvalidPassword_WhenLoggingInWithUsernameAndPassword_BadRequestIsReturned()
        {
            var request = new RegisterWithEmailAndPassword.Command
            {
                Email = "unique@email.com",
                FirstName = "Jozko",
                LastName = "Javorsky",
                Password = "Pass123###"
            };

            await NotAuthedClient.PostAsJsonAsync("api/auth/register", request);

            var authUser = await AssertDb.Users.SingleAsync(x => x.Email == request.Email);
            authUser.EmailConfirmed = true;
            await AssertDb.SaveChangesAsync();

            var response = await NotAuthedClient.PostAsJsonAsync("/api/auth/login", new { emailOrUsername = authUser.UserName, password = "InvalidPassword" });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsAsync<ErrorResponse>();
            content.Should().BeEquivalentTo(new
            {
                ErrorCode = "BadRequest",
                ErrorMessage = ErrorCodes.InvalidLoginCredentials
            });

        }
    }
}
