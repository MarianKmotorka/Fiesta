using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Fiesta.Application.Auth;
using Fiesta.Application.Common.Constants;
using Fiesta.Domain.Entities.Users;
using Fiesta.WebApi.Middleware.ExceptionHanlding;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Fiesta.IntegrationTests.Auth
{
    [Collection(nameof(FiestaAppFactory))]
    public class RegisterWithEmailAndPasswordTests : WebAppTestBase
    {
        public RegisterWithEmailAndPasswordTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenValidRequest_WhenRegisteringNewUser_AuthUserAndFiestaUserAreCreated()
        {
            var request = new RegisterWithEmailAndPassword.Command
            {
                Email = "unique@email.com",
                FirstName = "Jozko",
                LastName = "Javorsky",
                Password = "Pass123###"
            };

            var response = await NotAuthedClient.PostAsJsonAsync("api/auth/register", request);
            response.EnsureSuccessStatusCode();

            var authUser = await AssertDb.Users.SingleAsync(x => x.Email == request.Email);
            var fiestaUser = await AssertDb.FiestaUsers.SingleAsync(x => x.Email == request.Email);
            fiestaUser.FirstName.Should().Be(request.FirstName);
            fiestaUser.LastName.Should().Be(request.LastName);
            fiestaUser.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(3));
            fiestaUser.CreatedById.Should().BeNull();
        }

        [Fact]
        public async Task GivenInvalidRequest_WhenRegisteringNewUser_ErrorResponseIsReturend()
        {
            var request = new RegisterWithEmailAndPassword.Command
            {
                Email = "invalid",
                Password = "1234"
            };

            var response = await NotAuthedClient.PostAsJsonAsync("api/auth/register", request);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var errorResposne = await response.Content.ReadAsAsync<ErrorResponse>();
            errorResposne.Should().BeEquivalentTo(new
            {
                ErrorCode = "ValidationError",
                ErrorDetails = new object[]
                {
                    new
                    {
                        Code = ErrorCodes.InvalidEmailAddress,
                        PropertyName = "email",
                    },
                    new
                    {
                        Code = ErrorCodes.MinLength,
                        CustomState = JObject.FromObject(new { minLength = 6 }),
                        PropertyName="password"
                    },
                    new
                    {
                        Code = ErrorCodes.Required,
                        PropertyName = "firstName"
                    },
                    new
                    {
                        Code = ErrorCodes.Required,
                        PropertyName = "lastName"
                    }
                }
            });
        }

        [Fact]
        public async Task GivenDuplicateEmailRequest_WhenRegisteringNewUser_ErrorResponseIsReturend()
        {
            ArrangeDb.FiestaUsers.Add(new FiestaUser("duplicate@email.com"));
            await ArrangeDb.SaveChangesAsync();

            var request = new RegisterWithEmailAndPassword.Command
            {
                Email = "duplicate@email.com",
                FirstName = "Jozko",
                LastName = "Javorsky",
                Password = "Pass123###"
            };

            var response = await NotAuthedClient.PostAsJsonAsync("api/auth/register", request);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var errorResposne = await response.Content.ReadAsAsync<ErrorResponse>();
            errorResposne.Should().BeEquivalentTo(new
            {
                ErrorCode = "ValidationError",
                ErrorDetails = new object[]
                {
                    new
                    {
                        Code = ErrorCodes.MustBeUnique,
                        PropertyName = "email",
                    }
                }
            });
        }
    }
}
