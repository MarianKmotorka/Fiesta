using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Fiesta.Application.Common.Queries;
using Fiesta.Application.Features.Events;
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

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/get-attendees", new { });
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsAsync<QueryResponse<GetEventAttendees.AttendeeDto>>();
            content.Entries.Should().HaveCount(2);
            content.Entries.Should().BeEquivalentTo(new[]
            {
                new { attendee.Id },
                new { attendee2.Id }
            });
        }

        [Fact]
        public async Task GivenAttendeesInDb_WhenAttendeeGettingPrivateEventAttendees_ListIsReturned()
        {
            var attendee = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);
            var (_, organizer) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer, x => x.AccessibilityType = AccessibilityType.Private);
            @event.AddAttendee(attendee);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/get-attendees", new { });
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsAsync<QueryResponse<GetEventAttendees.AttendeeDto>>();
            content.Entries.Should().BeEquivalentTo(new[]
            {
                new { attendee.Id },
            });
        }

        [Fact]
        public async Task GivenAttendeesInDb_WhenFriendGettingFriendsOnlyEventAttendees_ListIsReturned()
        {
            var me = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);
            var (_, organizer) = ArrangeDb.SeedBasicUser();
            var (_, attendee) = ArrangeDb.SeedBasicUser();
            me.AddFriend(organizer);
            var @event = ArrangeDb.SeedEvent(organizer, x => x.AccessibilityType = AccessibilityType.FriendsOnly);
            @event.AddAttendee(attendee);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/get-attendees", new { });
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsAsync<QueryResponse<GetEventAttendees.AttendeeDto>>();
            content.Entries.Should().BeEquivalentTo(new[]
            {
                new { attendee.Id },
            });
        }

        [Fact]
        public async Task GivenAttendeesInDb_WhenNotAttendeeGettingPrivateEventAttendees_ForbiddenIsReturned()
        {
            var (_, organizer) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer, x => x.AccessibilityType = AccessibilityType.Private);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/get-attendees", new { });
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GivenAttendeesInDb_WhenNotFriendNorAttendeeGettingFriendsOnlyEventAttendees_ForbiddenIsReturned()
        {
            var (_, organizer) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer, x => x.AccessibilityType = AccessibilityType.FriendsOnly);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/get-attendees", new { });
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
