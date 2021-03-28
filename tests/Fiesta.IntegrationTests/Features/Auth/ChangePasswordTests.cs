using Fiesta.Application.Common.Constants;
using Fiesta.Application.Features.Auth;
using Fiesta.WebApi.Controllers;
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
    public class ChangePasswordTests : WebAppTestBase
    {
        public ChangePasswordTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenValidRequest_WhenChangePassword_PasswordIsChanged()
        {
            var registerRequest = new RegisterWithEmailAndPassword.Command
            {
                Email = "unique@email.com",
                FirstName = "Jozko",
                LastName = "Javorsky",
                Password = "Pass123###"
            };

            var registerResponse = await NotAuthedClient.PostAsJsonAsync("api/auth/register", registerRequest);

            registerResponse.EnsureSuccessStatusCode();

            var authUser = await ArrangeDb.Users.SingleAsync(x => x.Email == registerRequest.Email);
            authUser.EmailConfirmed = true;
            await ArrangeDb.SaveChangesAsync();

            var changePasswordRequest = new ChangePassword.Command
            {
                UserId = authUser.Id,
                CurrentPassword = registerRequest.Password,
                NewPassword = "Pass456###",
            };

            var changePasswordResponse = await NotAuthedClient.PostAsJsonAsync("api/auth/change-password", changePasswordRequest);

            changePasswordResponse.EnsureSuccessStatusCode();

            var loginRequest = new EmailPasswordRequest
            {
                EmailOrUsername = registerRequest.Email,
                Password = changePasswordRequest.NewPassword
            };

            var loginResponse = await NotAuthedClient.PostAsJsonAsync("api/auth/login", loginRequest);

            loginResponse.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GivenInvalidRequest_WhenChangePassword_ErrorResponseIsReturned()
        {
            var registerRequest = new RegisterWithEmailAndPassword.Command
            {
                Email = "unique@email.com",
                FirstName = "Jozko",
                LastName = "Javorsky",
                Password = "Pass123###"
            };

            var registerResponse = await NotAuthedClient.PostAsJsonAsync("api/auth/register", registerRequest);

            var authUser = await ArrangeDb.Users.SingleAsync(x => x.Email == registerRequest.Email);
            authUser.EmailConfirmed = true;
            await ArrangeDb.SaveChangesAsync();

            var changePasswordRequest = new ChangePassword.Command
            {
                UserId = authUser.Id,
                CurrentPassword = "BadPassword",
                NewPassword = "Pass456###",
            };

            var response = await NotAuthedClient.PostAsJsonAsync("api/auth/change-password", changePasswordRequest);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var errorResposne = await response.Content.ReadAsAsync<ErrorResponse>();
            errorResposne.Should().BeEquivalentTo(new
            {
                ErrorCode = "BadRequest",
                ErrorMessage = ErrorCodes.InvalidPassword
            });
        }
    }
}
