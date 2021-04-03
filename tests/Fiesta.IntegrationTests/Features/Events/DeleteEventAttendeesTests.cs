using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TestBase.Assets;
using Xunit;

namespace Fiesta.WebApi.Tests.Features.Events
{
    [Collection(nameof(FiestaAppFactory))]
    public class DeleteEventAttendeesTests : WebAppTestBase
    {
        public DeleteEventAttendeesTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenValidRequest_WhenDeletingAttendees_AttendeesAreRemoved()
        {
            var organizer = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);
            var (_, user1) = ArrangeDb.SeedBasicUser();
            var (_, user2) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer);
            @event.AddAttendee(user1);
            @event.AddAttendee(user2);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/attendees/delete", new { RemoveUserIds = new[] { user1.Id } });
            response.EnsureSuccessStatusCode();

            var eventDb = await AssertDb.Events.Include(x => x.Attendees).SingleAsync(x => x.Id == @event.Id);
            eventDb.Attendees.Should().BeEquivalentTo(new[]
            {
                new{  AttendeeId = user2.Id, EventId=@event.Id },
            });
        }

        [Fact]
        public async Task GivenUserWhoIsNotOrganizer_WhenDeletingAttendees_ForbiddenIsReturned()
        {
            var (_, organizer) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/attendees/delete", new { RemoveUserIds = new[] { "some_id" } });
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}


