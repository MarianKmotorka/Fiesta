using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TestBase.Assets;
using Xunit;


namespace Fiesta.WebApi.Tests.Features.Events
{
    [Collection(nameof(TestCollection))]
    public class ReplyToEventJoinRequest : WebAppTestBase
    {
        public ReplyToEventJoinRequest(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GiveExistingJoinRequest_WhenAcceptingTheRequest_EventAttendeeIsAddedAndJoinRequestIsRemoved()
        {
            var organizer = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);
            var (_, requestedUser) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer);
            @event.AddJoinRequest(requestedUser);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/join-requests/reply", new { Accepted = true, UserId = requestedUser.Id });
            response.EnsureSuccessStatusCode();

            var eventDb = await AssertDb.Events
                .Include(x => x.JoinRequests)
                .Include(x => x.Attendees)
                .SingleAsync(x => x.Id == @event.Id);

            eventDb.JoinRequests.Should().BeEmpty();
            eventDb.Attendees.Should().BeEquivalentTo(new[]
            {
                new{ AttendeeId = requestedUser.Id, EventId = @event.Id },
            });
        }

        [Fact]
        public async Task GiveExistingJoinRequest_WhenRejectingTheRequest_JoinRequestIsRemoved()
        {
            var organizer = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);
            var (_, requestedUser) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer);
            @event.AddJoinRequest(requestedUser);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/join-requests/reply", new { Accepted = false, UserId = requestedUser.Id });
            response.EnsureSuccessStatusCode();

            var eventDb = await AssertDb.Events
                .Include(x => x.JoinRequests)
                .Include(x => x.Attendees)
                .SingleAsync(x => x.Id == @event.Id);

            eventDb.JoinRequests.Should().BeEmpty();
            eventDb.Attendees.Should().BeEmpty();
        }

        [Fact]
        public async Task GiveExistingJoinRequest_WhenNotOrganizerIsReplying_ForbiddenIsReturned()
        {
            var requestedUser = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);
            var (_, organizer) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer);
            @event.AddJoinRequest(requestedUser);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/join-requests/reply", new { Accepted = true, UserId = requestedUser.Id });
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}

