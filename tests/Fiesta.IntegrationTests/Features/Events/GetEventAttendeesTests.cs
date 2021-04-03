using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Fiesta.Application.Common.Queries;
using Fiesta.Application.Features.Common;
using Fiesta.Domain.Entities.Events;
using FluentAssertions;
using TestBase.Assets;
using Xunit;

namespace Fiesta.WebApi.Tests.Features.Events
{
    [Collection(nameof(FiestaAppFactory))]
    public class GetEventAttendeesTests : WebAppTestBase
    {
        public GetEventAttendeesTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenAttendeesInDb_WhenCallingGetAttendees_ListIsReturned()
        {
            var organizer = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);
            var (_, attendee) = ArrangeDb.SeedBasicUser();
            var (_, attendee2) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer);
            @event.AddAttendee(attendee);
            @event.AddAttendee(attendee2);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/attendees/query", new { });
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsAsync<QueryResponse<UserDto>>();
            content.Entries.Should().HaveCount(2);
            content.Entries.Should().BeEquivalentTo(new[]
            {
                new { attendee.Id },
                new { attendee2.Id }
            });
        }

        [Theory]
        [InlineData(AccessibilityType.Public, HttpStatusCode.OK)]
        [InlineData(AccessibilityType.FriendsOnly, HttpStatusCode.Forbidden)]
        [InlineData(AccessibilityType.Private, HttpStatusCode.Forbidden)]
        public async Task GivenUserIsNotAttendeeNorFriend_WhenGettingEventAttendees_ExpectedResponseIsReturned(AccessibilityType accessibility, HttpStatusCode statusCode)
        {
            var (_, organizer) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer, x => x.AccessibilityType = accessibility);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/attendees/query", new { });
            response.StatusCode.Should().Be(statusCode);
        }

        [Theory]
        [InlineData(AccessibilityType.Public, HttpStatusCode.OK)]
        [InlineData(AccessibilityType.FriendsOnly, HttpStatusCode.OK)]
        [InlineData(AccessibilityType.Private, HttpStatusCode.OK)]
        public async Task GivenUserIsAttendee_WhenGettingEventAttendees_ExpectedResponseIsReturned(AccessibilityType accessibility, HttpStatusCode statusCode)
        {
            var me = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);
            var (_, organizer) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer, x => x.AccessibilityType = accessibility);
            @event.AddAttendee(me);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/attendees/query", new { });
            response.StatusCode.Should().Be(statusCode);
        }

        [Theory]
        [InlineData(AccessibilityType.Public, HttpStatusCode.OK)]
        [InlineData(AccessibilityType.FriendsOnly, HttpStatusCode.OK)]
        [InlineData(AccessibilityType.Private, HttpStatusCode.Forbidden)]
        public async Task GivenUserIsFriend_WhenGettingEventAttendees_ExpectedResponseIsReturned(AccessibilityType accessibility, HttpStatusCode statusCode)
        {
            var me = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);
            var (_, organizer) = ArrangeDb.SeedBasicUser();
            organizer.AddFriend(me);
            var @event = ArrangeDb.SeedEvent(organizer, x => x.AccessibilityType = accessibility);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/attendees/query", new { });
            response.StatusCode.Should().Be(statusCode);
        }
    }
}
