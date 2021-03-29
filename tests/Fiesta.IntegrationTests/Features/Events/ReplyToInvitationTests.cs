using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TestBase.Assets;
using Xunit;


namespace Fiesta.WebApi.Tests.Features.Events
{
    [Collection(nameof(FiestaAppFactory))]
    public class ReplyToInvitationTests : WebAppTestBase
    {
        public ReplyToInvitationTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GiveEXistingInvitation_WhenAcceptingTheInvitation_EventAttendeeIsAddedAndInvitationIsRemoved()
        {
            var invitee = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);
            var (_, organizer) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer);
            @event.AddInvitation(invitee);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/invitation-reply", new { Accepted = true });
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
        public async Task GiveEXistingInvitation_WhenRejectingTheInvitation_InvitationIsRemoved()
        {
            var invitee = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);
            var (_, organizer) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer);
            @event.AddInvitation(invitee);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/invitation-reply", new { Accepted = false });
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
