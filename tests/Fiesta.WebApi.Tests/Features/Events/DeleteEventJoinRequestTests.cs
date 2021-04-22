using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TestBase.Assets;
using Xunit;

namespace Fiesta.WebApi.Tests.Features.Events
{
    [Collection(nameof(TestCollection))]
    public class DeleteEventJoinRequestTests : WebAppTestBase
    {
        public DeleteEventJoinRequestTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenValidRequest_WhenDeletingJoinRequest_JoinReqeustIsRemoved()
        {
            var requestedUser = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);
            var (_, organizer) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer);
            @event.AddJoinRequest(requestedUser);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/join-requests/delete", new { });
            response.EnsureSuccessStatusCode();

            var eventDb = await AssertDb.Events.Include(x => x.JoinRequests).SingleAsync(x => x.Id == @event.Id);
            eventDb.JoinRequests.Should().BeEmpty();
        }
    }
}


