﻿using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Fiesta.Application.Features.Common;
using Fiesta.Application.Features.Events;
using Fiesta.Application.Features.Events.Common;
using Fiesta.Domain.Entities.Events;
using FluentAssertions;
using TestBase.Assets;
using Xunit;

namespace Fiesta.WebApi.Tests.Features.Events
{
    [Collection(nameof(TestCollection))]
    public class GetEventDetailTests : WebAppTestBase
    {
        public GetEventDetailTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenEventStoredInDb_WhenGettingDetail_DetailReturned()
        {
            var organizer = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);
            var (_, attendee) = ArrangeDb.SeedBasicUser();
            var (_, attendee2) = ArrangeDb.SeedBasicUser();
            var (_, invited) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer);
            @event.AddAttendee(attendee);
            @event.AddAttendee(attendee2);
            @event.AddInvitations(invited);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.GetAsync($"/api/events/{@event.Id}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsAsync<GetEventDetail.Response>();
            content.Should().BeEquivalentTo(new GetEventDetail.Response
            {
                Id = @event.Id,
                Name = @event.Name,
                AccessibilityType = @event.AccessibilityType,
                AttendeesCount = 2,
                InvitationsCount = 1,
                EndDate = @event.EndDate,
                StartDate = @event.StartDate,
                BannerUrl = @event.BannerUrl,
                Capacity = @event.Capacity,
                Location = LocationDto.Map(@event.Location),
                Organizer = new UserDto
                {
                    Id = organizer.Id,
                    Username = organizer.Username,
                    PictureUrl = organizer.PictureUrl
                }
            });
        }

        [Theory]
        [InlineData(AccessibilityType.Public, HttpStatusCode.OK)]
        [InlineData(AccessibilityType.FriendsOnly, HttpStatusCode.Forbidden)]
        [InlineData(AccessibilityType.Private, HttpStatusCode.Forbidden)]
        public async Task GivenUserIsNotAttendeeNorFriend_WhenGettingDetail_ExpectedResponseIsReturned(AccessibilityType accessibility, HttpStatusCode statusCode)
        {
            var (_, organizer) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer, x => x.AccessibilityType = accessibility);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.GetAsync($"/api/events/{@event.Id}");
            response.StatusCode.Should().Be(statusCode);
        }

        [Theory]
        [InlineData(AccessibilityType.Public, HttpStatusCode.OK)]
        [InlineData(AccessibilityType.FriendsOnly, HttpStatusCode.OK)]
        [InlineData(AccessibilityType.Private, HttpStatusCode.OK)]
        public async Task GivenUserIsAttendee_WhenGettingDetail_ExpectedResponseIsReturned(AccessibilityType accessibility, HttpStatusCode statusCode)
        {
            var me = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);
            var (_, organizer) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer, x => x.AccessibilityType = accessibility);
            @event.AddAttendee(me);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.GetAsync($"/api/events/{@event.Id}");
            response.StatusCode.Should().Be(statusCode);
        }

        [Theory]
        [InlineData(AccessibilityType.Public, HttpStatusCode.OK)]
        [InlineData(AccessibilityType.FriendsOnly, HttpStatusCode.OK)]
        [InlineData(AccessibilityType.Private, HttpStatusCode.OK)]
        public async Task GivenUserIsInvited_WhenGettingDetail_ExpectedResponseIsReturned(AccessibilityType accessibility, HttpStatusCode statusCode)
        {
            var me = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);
            var (_, organizer) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer, x => x.AccessibilityType = accessibility);
            @event.AddAttendee(me);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.GetAsync($"/api/events/{@event.Id}");
            response.StatusCode.Should().Be(statusCode);
        }

        [Theory]
        [InlineData(AccessibilityType.Public, HttpStatusCode.OK)]
        [InlineData(AccessibilityType.FriendsOnly, HttpStatusCode.OK)]
        [InlineData(AccessibilityType.Private, HttpStatusCode.Forbidden)]
        public async Task GivenUserIsFriend_WhenGettingDetail_ExpectedResponseIsReturned(AccessibilityType accessibility, HttpStatusCode statusCode)
        {
            var me = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);
            var (_, organizer) = ArrangeDb.SeedBasicUser();
            organizer.AddFriend(me);
            var @event = ArrangeDb.SeedEvent(organizer, x => x.AccessibilityType = accessibility);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.GetAsync($"/api/events/{@event.Id}");
            response.StatusCode.Should().Be(statusCode);
        }
    }
}
