using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Features.Events.CommonDtos;
using Fiesta.Domain.Entities.Events;
using Fiesta.WebApi.Middleware.ExceptionHanlding;
using FluentAssertions;
using Xunit;

namespace Fiesta.WebApi.Tests.Features.Events
{
    [Collection(nameof(FiestaAppFactory))]
    public class CreateEventTests : WebAppTestBase
    {
        public CreateEventTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenValidRequest_WhenCreateEvent_EventIsCreated()
        {
            var location = new LocationDto()
            {
                Latitude = 0,
                Longitude = 0,
                Street = "",
                StreetNumber = "",
                Premise = "",
                City = "",
                State = "",
                AdministrativeAreaLevel1 = "",
                AdministrativeAreaLevel2 = "",
                PostalCode = "",
                GoogleMapsUrl = ""
            };

            var organizedEvent = new
            {
                name = "Welding competition",
                startDate = DateTime.Now,
                endDate = DateTime.Now.AddDays(1),
                accessibilityType = AccessibilityType.Public,
                capacity = 10,
                location
            };

            var createResponse = await Client.PostAsJsonAsync("/api/events/create", organizedEvent);
            createResponse.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GivenInvalidRequest_WhenCreateEvent_BadRequestIsReturned()
        {
            var location = new LocationDto()
            {
                Latitude = 0,
                Longitude = 0,
                Street = "",
                StreetNumber = "",
                Premise = "",
                City = "",
                State = "",
                AdministrativeAreaLevel1 = "",
                AdministrativeAreaLevel2 = "",
                PostalCode = "",
                GoogleMapsUrl = ""
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

            var createResponse = await Client.PostAsJsonAsync("/api/events/create", organizedEvent);
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
                Street = "",
                StreetNumber = "",
                Premise = "",
                City = "",
                State = "",
                AdministrativeAreaLevel1 = "",
                AdministrativeAreaLevel2 = "",
                PostalCode = "",
                GoogleMapsUrl = ""
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

            var createResponse = await Client.PostAsJsonAsync("/api/events/create", organizedEvent);
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
    }
}
