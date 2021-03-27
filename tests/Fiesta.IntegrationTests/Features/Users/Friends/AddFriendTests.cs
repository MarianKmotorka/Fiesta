using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Domain.Entities.Users;
using Fiesta.IntegrationTests;
using Fiesta.WebApi.Middleware.ExceptionHanlding;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TestBase.Assets;
using Xunit;

namespace Fiesta.WebApi.Tests.Features.Users.Friends
{
    [Collection(nameof(FiestaAppFactory))]
    public class AddFriendTests : WebAppTestBase
    {
        public AddFriendTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenValidRequest_WhenConfirmingFriendRequest_FriendIsAdded()
        {
            var (authFriend, fiestaFriend) = ArrangeDb.SeedBasicUser();
            var fiestaUser = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);

            await ArrangeDb.FriendRequests.AddAsync(new FriendRequest(fiestaUser, fiestaFriend));
            await ArrangeDb.SaveChangesAsync();

            using var client = CreateClientForUser(authFriend);

            var response = await client.PostAsJsonAsync("api/friends/confirm-request", new { friendId = LoggedInUserId });

            response.EnsureSuccessStatusCode();

            var friendRequest = await AssertDb.FriendRequests.SingleOrDefaultAsync(x => x.FromId == LoggedInUserId && x.ToId == fiestaFriend.Id);

            friendRequest.Should().BeNull();

            var friendShip1 = await AssertDb.UserFriends.SingleOrDefaultAsync(x => x.UserId == LoggedInUserId && x.FriendId == fiestaFriend.Id);
            var friendShip2 = await AssertDb.UserFriends.SingleOrDefaultAsync(x => x.UserId == fiestaFriend.Id && x.FriendId == LoggedInUserId);

            friendShip1.Should().NotBeNull();
            friendShip2.Should().NotBeNull();
        }

        [Fact]
        public async Task GivenInvalidId_WhenConfirmingFriendRequest_BadRequestIsReturned()
        {
            var (authFriend, fiestaFriend) = ArrangeDb.SeedBasicUser();
            var fiestaUser = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);

            await ArrangeDb.FriendRequests.AddAsync(new FriendRequest(fiestaUser, fiestaFriend));
            await ArrangeDb.SaveChangesAsync();

            using var client = CreateClientForUser(authFriend);

            var response = await client.PostAsJsonAsync("api/friends/confirm-request", new { friendId = "InvalidId" });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var errorResposne = await response.Content.ReadAsAsync<ErrorResponse>();
            errorResposne.Should().BeEquivalentTo(new
            {
                ErrorCode = "ValidationError",
                ErrorDetails = new object[]
                {
                    new
                    {
                        Code = ErrorCodes.DoesNotExist,
                        PropertyName="friendId"
                    }
                }
            });
        }

        [Fact]
        public async Task GivenSenderConfirmingRequest_WhenConfirmingFriendRequest_BadRequestIsReturned()
        {
            var (authFriend, fiestaFriend) = ArrangeDb.SeedBasicUser();
            var fiestaUser = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);

            await ArrangeDb.FriendRequests.AddAsync(new FriendRequest(fiestaUser, fiestaFriend));
            await ArrangeDb.SaveChangesAsync();

            using var client = CreateClientForUser(authFriend);

            var response = await Client.PostAsJsonAsync("api/friends/confirm-request", new { friendId = fiestaFriend.Id });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var errorResposne = await response.Content.ReadAsAsync<ErrorResponse>();
            errorResposne.Should().BeEquivalentTo(new
            {
                ErrorCode = "BadRequest",
                ErrorMessage = "Friend request does not exist",
            });
        }
    }
}
