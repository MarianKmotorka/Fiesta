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
    public class DeleteEventInvitationsTests : WebAppTestBase
    {
        public DeleteEventInvitationsTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenValidRequest_WhenDeletingInvites_InvitesAreRemoved()
        {
            var organizer = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);
            var (_, user1) = ArrangeDb.SeedBasicUser();
            var (_, user2) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer);
            @event.AddInvitations(user1);
            @event.AddInvitations(user2);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/invitations/delete", new { RemoveUserIds = new[] { user1.Id } });
            response.EnsureSuccessStatusCode();

            var eventDb = await AssertDb.Events.Include(x => x.Invitations).SingleAsync(x => x.Id == @event.Id);
            eventDb.Invitations.Should().BeEquivalentTo(new[]
            {
                new{ InviterId = organizer.Id, InviteeId = user2.Id, EventId=@event.Id },
            });
        }

        [Fact]
        public async Task GivenUserWhoIsNotOrganizer_WhenDeletingInvites_ForbiddenIsReturned()
        {
            var (_, organizer) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/invitations/delete", new { RemoveUserIds = new[] { "some_id" } });
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}

