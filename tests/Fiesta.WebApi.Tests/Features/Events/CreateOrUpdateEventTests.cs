using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Features.Events.Common;
using Fiesta.Application.Features.Events.CreateOrUpdate;
using Fiesta.Domain.Entities.Events;
using Fiesta.WebApi.Middleware.ExceptionHanlding;
using FluentAssertions;
using TestBase.Assets;
using TestBase.Helpers;
using Xunit;

namespace Fiesta.WebApi.Tests.Features.Events
{
    [Collection(nameof(TestCollection))]
    public class CreateOrUpdateEventTests : WebAppTestBase
    {
        public CreateOrUpdateEventTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenValidRequest_WhenCreateEvent_EventIsCreated()
        {
            var location = new LocationDto()
            {
                Latitude = 0,
                Longitude = 0,
                City = "Oxford",
                State = "Italy",
            };

            var request = new CreateOrUpdateEvent.Command
            {
                Name = "Welding competition",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                AccessibilityType = AccessibilityType.Public,
                Capacity = 10,
                Location = location,
                Description = "Description",
            };

            var createResponse = await Client.PostAsJsonAsync("/api/events", request);
            createResponse.EnsureSuccessStatusCode();
            var content = await createResponse.Content.ReadAsAsync<CreateOrUpdateEvent.Response>();

            var eventDb = await AssertDb.Events.FindAsync(content.Id);
            eventDb.Should().BeEquivalentTo(new
            {
                content.Id,
                request.AccessibilityType,
                request.StartDate,
                request.EndDate,
                request.Capacity,
                request.Description,
                Location = new
                {
                    location.City,
                    location.State,
                    location.Latitude,
                    location.Longitude,
                    GoogleMapsUrl = $"https://www.google.com/maps/search/?api=1&query={location.Latitude},{location.Longitude}"
                }
            });
        }

        [Fact]
        public async Task GivenInvalidRequest_WhenCreateEvent_BadRequestIsReturned()
        {
            var location = new LocationDto()
            {
                Latitude = 0,
                Longitude = 0,
            };

            var organizedEvent = new
            {
                name = "Welding competition",
                startDate = DateTime.Now,
                endDate = DateTime.Now.AddDays(1),
                accessibilityType = 8,
                capacity = 10,
                location
            };

            var createResponse = await Client.PostAsJsonAsync("/api/events", organizedEvent);
            createResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var errorResposne = await createResponse.Content.ReadAsAsync<ErrorResponse>();
            errorResposne.Should().BeEquivalentTo(new
            {
                ErrorCode = "ValidationError",
                ErrorDetails = new object[]
                {
                    new
                    {
                        Code = ErrorCodes.InvalidEnumValue,
                        PropertyName="accessibilityType"
                    }
                }
            });
        }

        [Fact]
        public async Task GivenInvalidLocation_WhenCreateEvent_BadRequestIsReturned()
        {
            var location = new LocationDto()
            {
                Latitude = 1000,
                Longitude = -1000,
            };

            var organizedEvent = new
            {
                name = "Welding competition",
                startDate = DateTime.Now,
                endDate = DateTime.Now.AddDays(1),
                accessibilityType = AccessibilityType.FriendsOnly,
                capacity = 10,
                location
            };

            var createResponse = await Client.PostAsJsonAsync("/api/events", organizedEvent);
            createResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var errorResposne = await createResponse.Content.ReadAsAsync<ErrorResponse>();
            errorResposne.Should().BeEquivalentTo(new
            {
                ErrorCode = "ValidationError",
                ErrorDetails = new object[]
                {
                    new
                    {
                        Code = ErrorCodes.InvalidLatitudeOrLongitude,
                        PropertyName="location"
                    }
                }
            });
        }

        [Fact]
        public async Task GivenValidRequest_WhenUpdatingEvent_EventIsUpdated()
        {
            var organizer = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);
            var existingEvent = ArrangeDb.SeedEvent(organizer);
            await ArrangeDb.SaveChangesAsync();

            var location = new LocationDto()
            {
                Latitude = 30,
                Longitude = 40,
                Street = "newStreet",
                StreetNumber = "newStreetNum",
                City = "newCity",
                State = "newState",
            };

            var request = new CreateOrUpdateEvent.Command
            {
                Id = existingEvent.Id,
                Name = "Welding competition EXTRA",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(10),
                AccessibilityType = AccessibilityType.Private,
                Capacity = 100,
                Location = location
            };

            var response = await Client.PatchAsJsonAsync($"/api/events/{existingEvent.Id}", request);
            response.EnsureSuccessStatusCode();

            var eventDb = await AssertDb.Events.FindAsync(request.Id);
            eventDb.Should().BeEquivalentTo(new
            {
                existingEvent.Id,
                request.AccessibilityType,
                request.StartDate,
                request.EndDate,
                request.Capacity,
                request.Description,
                request.Name,
                Location = new
                {
                    location.City,
                    location.State,
                    location.Latitude,
                    location.Longitude,
                    location.Street,
                    location.StreetNumber,
                    GoogleMapsUrl = $"https://www.google.com/maps/search/?api=1&query={location.Latitude},{location.Longitude}"
                }
            });
        }
    }
}
