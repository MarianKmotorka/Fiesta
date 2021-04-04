using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TestBase.Assets;
using Xunit;


namespace Fiesta.WebApi.Tests.Features.Events
{
    [Collection(nameof(FiestaAppFactory))]
    public class ReplyToEventInvitationTests : WebAppTestBase
    {
        public ReplyToEventInvitationTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GiveExistingInvitation_WhenAcceptingTheInvitation_EventAttendeeIsAddedAndInvitationIsRemoved()
        {
            var invitee = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);
            var (_, organizer) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer);
            @event.AddInvitations(invitee);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/invitations/reply", new { Accepted = true });
            response.EnsureSuccessStatusCode();

            var eventDb = await AssertDb.Events
                .Include(x => x.Invitations)
                .Include(x => x.Attendees)
                .SingleAsync(x => x.Id == @event.Id);

            eventDb.Invitations.Should().BeEmpty();
            eventDb.Attendees.Should().BeEquivalentTo(new[]
            {
                new{ AttendeeId = invitee.Id, EventId=@event.Id },
            });
        }

        [Fact]
        public async Task GiveExistingInvitation_WhenRejectingTheInvitation_InvitationIsRemoved()
        {
            var invitee = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);
            var (_, organizer) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer);
            @event.AddInvitations(invitee);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/invitations/reply", new { Accepted = false });
            response.EnsureSuccessStatusCode();

            var eventDb = await AssertDb.Events
                .Include(x => x.Invitations)
                .Include(x => x.Attendees)
                .SingleAsync(x => x.Id == @event.Id);

            eventDb.Invitations.Should().BeEmpty();
            eventDb.Attendees.Should().BeEmpty();
        }
    }
}
