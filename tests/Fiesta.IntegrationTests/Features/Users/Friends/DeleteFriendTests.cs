using System.Threading.Tasks;
using Fiesta.Domain.Entities.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TestBase.Assets;
using Xunit;

namespace Fiesta.WebApi.Tests.Features.Users.Friends
{
    [Collection(nameof(TestCollection))]
    public class DeleteFriendTests : WebAppTestBase
    {
        public DeleteFriendTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenValidRequest_WhenDeletingFriendFromOneSide_FriendIsDeleted()
        {
            var (_, fiestaFriend) = ArrangeDb.SeedBasicUser();
            var fiestaUser = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);

            await ArrangeDb.UserFriends.AddAsync(new UserFriend(fiestaUser, fiestaFriend));
            await ArrangeDb.UserFriends.AddAsync(new UserFriend(fiestaFriend, fiestaUser));
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.DeleteAsync($"api/friends/{fiestaFriend.Id}");

            response.EnsureSuccessStatusCode();

            var friendShip1 = await AssertDb.UserFriends.SingleOrDefaultAsync(x => x.UserId == LoggedInUserId && x.FriendId == fiestaFriend.Id);
            var friendShip2 = await AssertDb.UserFriends.SingleOrDefaultAsync(x => x.UserId == fiestaFriend.Id && x.FriendId == LoggedInUserId);

            friendShip1.Should().BeNull();
            friendShip2.Should().BeNull();
        }

        [Fact]
        public async Task GivenValidRequest_WhenDeletingFriendFromOtherSide_FriendIsDeleted()
        {
            var (authFriend, fiestaFriend) = ArrangeDb.SeedBasicUser();
            var fiestaUser = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);

            await ArrangeDb.UserFriends.AddAsync(new UserFriend(fiestaUser, fiestaFriend));
            await ArrangeDb.UserFriends.AddAsync(new UserFriend(fiestaFriend, fiestaUser));
            await ArrangeDb.SaveChangesAsync();

            using var client = CreateClientForUser(authFriend);

            var response = await client.DeleteAsync($"api/friends/{LoggedInUserId}");

            response.EnsureSuccessStatusCode();

            var friendShip1 = await AssertDb.UserFriends.SingleOrDefaultAsync(x => x.UserId == LoggedInUserId && x.FriendId == fiestaFriend.Id);
            var friendShip2 = await AssertDb.UserFriends.SingleOrDefaultAsync(x => x.UserId == fiestaFriend.Id && x.FriendId == LoggedInUserId);

            friendShip1.Should().BeNull();
            friendShip2.Should().BeNull();
        }
    }
}
