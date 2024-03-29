﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Features.Auth;
using Fiesta.Infrastracture.Auth;
using Fiesta.WebApi.Middleware.ExceptionHanlding;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Fiesta.WebApi.Tests.Features.Auth
{
    [Collection(nameof(TestCollection))]
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
            authUser.EmailConfirmed.Should().BeFalse();
            authUser.Role.Should().Be(FiestaRoleEnum.BasicUser);

            var fiestaUser = await AssertDb.FiestaUsers.SingleAsync(x => x.Email == request.Email);
            fiestaUser.FirstName.Should().Be(request.FirstName);
            fiestaUser.LastName.Should().Be(request.LastName);
            fiestaUser.CreatedOnUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(3));
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
            ArrangeDb.Users.Add(new AuthUser("duplicate@email.com", AuthProviderEnum.EmailAndPassword, "Duplicate"));
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

        [Fact]
        public async Task GivenExistingUserWithConnectedGoogleAccount_WhenRegisteringWithSameEmailAsGoogleAccount_ErrorResponseIsReturend()
        {
            var user = new AuthUser("unique@email.com", AuthProviderEnum.EmailAndPassword, "Unique");
            user.AddGoogleAuthProvider("duplicate@email.com");
            ArrangeDb.Users.Add(user);
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
