﻿using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Fiesta.Application.Auth;
using Fiesta.Application.Auth.CommonDtos;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Infrastracture.Persistence;
using Fiesta.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Fiesta.IntegrationTests
{
    public abstract class WebAppTestBase : IDisposable
    {
        public HttpClient Client { get; }
        public HttpClient NotAuthedClient { get; }
        public FiestaDbContext ArrangeDb { get; }
        public FiestaDbContext ActDb { get; }
        public FiestaDbContext AssertDb { get; }

        public WebAppTestBase(FiestaAppFactory factory)
        {
            Client = factory.CreateClient();
            NotAuthedClient = factory.CreateClient();

            ActDb = CreateDb();
            ArrangeDb = CreateDb();
            AssertDb = CreateDb();

            Authenticate(Client);
        }

        private static FiestaDbContext CreateDb()
        {
            var optionsBuilder = new DbContextOptionsBuilder<FiestaDbContext>().UseInMemoryDatabase(FiestaDbContext.TestDbName);
            return new FiestaDbContext(optionsBuilder.Options, new FakeCurrentUserServiceOnlyForDbContextCreation());
        }

        private void Authenticate(HttpClient client)
        {
            var command = new RegisterWithEmailAndPassword.Command
            {
                Email = "admin@fiesta.com",
                FirstName = "Admin",
                LastName = "Fiesta",
                Password = "Password123"
            };

            var response = Client.PostAsJsonAsync("api/auth/register", command).Result;
            var err = response.Content.ReadAsStringAsync().Result;
            response.EnsureSuccessStatusCode();

            var authUser = ArrangeDb.Users.Single(x => x.Email == command.Email);
            authUser.EmailConfirmed = true;
            authUser.Role = FiestaRole.Admin;
            ArrangeDb.SaveChanges();

            response = Client.PostAsJsonAsync("api/auth/login", new EmailPasswordRequest { Email = command.Email, Password = command.Password }).Result;
            response.EnsureSuccessStatusCode();
            var accessToken = response.Content.ReadAsAsync<AuthResponse>().Result.AccessToken;

            // TODO: Create accessToken with long expiration for testing purposes
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public void Dispose()
        {
            ArrangeDb.Database.EnsureDeleted();

            ArrangeDb.Dispose();
            ActDb.Dispose();
            AssertDb.Dispose();
        }
    }

    /// <summary>
    /// Do not use. It is only for DbContext creation purposes.
    /// </summary>
    public class FakeCurrentUserServiceOnlyForDbContextCreation : ICurrentUserService
    {
        public string UserId => throw new NotImplementedException();

        public FiestaRole Role => throw new NotImplementedException();

        public HttpContext HttpContext => throw new NotImplementedException();
    }

    [CollectionDefinition(nameof(FiestaAppFactory))]
    public class WebAppCollection : ICollectionFixture<FiestaAppFactory>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}