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
    public class UnsendFriendRequestTests : WebAppTestBase
    {
        public UnsendFriendRequestTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenValidRequest_WhenUnsendingFriendRequest_RequestIsUnsent()
        {
            var (_, fiestaFriend) = ArrangeDb.SeedBasicUser();
            await ArrangeDb.SaveChangesAsync();

            await Client.PostAsJsonAsync("api/friends/send-request", new { friendId = fiestaFriend.Id });

            var friendRequest = await ArrangeDb.FriendRequests.SingleOrDefaultAsync(x => x.FromId == LoggedInUserId && x.ToId == fiestaFriend.Id);

            friendRequest.Should().NotBeNull();

            var response = await Client.PostAsJsonAsync("api/friends/unsend-request", new { friendId = fiestaFriend.Id });

            response.EnsureSuccessStatusCode();

            friendRequest = await AssertDb.FriendRequests.SingleOrDefaultAsync(x => x.FromId == LoggedInUserId && x.ToId == fiestaFriend.Id);

            friendRequest.Should().BeNull();
        }
    }
}
