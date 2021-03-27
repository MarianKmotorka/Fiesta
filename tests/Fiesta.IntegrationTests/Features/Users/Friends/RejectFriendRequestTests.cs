using System.Net.Http;
using System.Threading.Tasks;
using Fiesta.IntegrationTests;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TestBase.Assets;
using Xunit;

namespace Fiesta.WebApi.Tests.Features.Users.Friends
{
    [Collection(nameof(FiestaAppFactory))]
    public class RejectFriendRequestTests : WebAppTestBase
    {
        public RejectFriendRequestTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenValidRequest_WhenRejectingFriendRequest_RequestIsRejected()
        {
            var (authFriend, fiestaFriend) = ArrangeDb.SeedBasicUser();
            await ArrangeDb.SaveChangesAsync();

            await Client.PostAsJsonAsync("api/friends/send-request", new { friendId = fiestaFriend.Id });

            var friendRequest = await ArrangeDb.FriendRequests.SingleOrDefaultAsync(x => x.FromId == LoggedInUserId && x.ToId == fiestaFriend.Id);

            friendRequest.Should().NotBeNull();

            using var client = CreateClientForUser(authFriend);

            var response = await client.PostAsJsonAsync("api/friends/reject-request", new { friendId = LoggedInUserId });

            response.EnsureSuccessStatusCode();

            friendRequest = await AssertDb.FriendRequests.SingleOrDefaultAsync(x => x.FromId == LoggedInUserId && x.ToId == fiestaFriend.Id);

            friendRequest.Should().BeNull();
        }
    }
}
