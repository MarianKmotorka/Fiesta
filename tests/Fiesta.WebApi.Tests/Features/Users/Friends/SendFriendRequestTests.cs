using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.WebApi.Middleware.ExceptionHanlding;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TestBase.Assets;
using Xunit;

namespace Fiesta.WebApi.Tests.Features.Users.Friends
{
    [Collection(nameof(TestCollection))]
    public class SendFriendRequestTests : WebAppTestBase
    {
        public SendFriendRequestTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenValidRequest_WhenSendingFriendRequest_RequestIsSent()
        {
            var (_, fiestaFriend) = ArrangeDb.SeedBasicUser();
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync("api/friends/send-request", new { friendId = fiestaFriend.Id });

            response.EnsureSuccessStatusCode();

            var friendRequest = await AssertDb.FriendRequests.SingleOrDefaultAsync(x => x.FromId == LoggedInUserId && x.ToId == fiestaFriend.Id);

            friendRequest.Should().NotBeNull();
            friendRequest.FromId.Should().BeEquivalentTo(LoggedInUserId);
            friendRequest.ToId.Should().BeEquivalentTo(fiestaFriend.Id);
        }

        [Fact]
        public async Task GivenInvalidId_WhenSendingFriendRequest_BadRequestIsReturned()
        {
            var response = await Client.PostAsJsonAsync("api/friends/send-request", new { friendId = "InvalidId" });

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
        public async Task GivenSenderAndReceiverIdentical_WhenSendingFriendRequest_BadRequestIsReturned()
        {
            var response = await Client.PostAsJsonAsync("api/friends/send-request", new { friendId = LoggedInUserId });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var errorResposne = await response.Content.ReadAsAsync<ErrorResponse>();
            errorResposne.Should().BeEquivalentTo(new
            {
                ErrorCode = "ValidationError",
                ErrorDetails = new object[]
                {
                    new
                    {
                        Code = ErrorCodes.SenderAndReceiverIdentical,
                        PropertyName="friendId"
                    }
                }
            });
        }
    }
}
