using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Features.Events.Comments;
using Fiesta.Application.Features.Events.Comments.Common;
using Fiesta.Domain.Entities.Events;
using Fiesta.WebApi.Middleware.ExceptionHanlding;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TestBase.Assets;
using Xunit;

namespace Fiesta.WebApi.Tests.Features.Events.Comments
{
    [Collection(nameof(TestCollection))]
    public class CreateCommentTests : WebAppTestBase
    {
        public CreateCommentTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenValidRequest_WhenCreatingComment_CommentIsCreated()
        {
            var (authUser, user) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(user);
            await ArrangeDb.SaveChangesAsync();
            var request = new CreateComment.Command { Text = "New comment" };

            using var client = CreateClientForUser(authUser);
            var response = await client.PostAsJsonAsync($"/api/events/{@event.Id}/comments", request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsAsync<CommentDto>();
            content.Should().BeEquivalentTo(new
            {
                Text = request.Text,
                IsEdited = content.IsEdited,
                ParentId = content.ParentId,
                ReplyCount = 0,
                Sender = new
                {
                    Id = user.Id,
                    Username = user.Username,
                    PictureUrl = user.PictureUrl,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                },
            });

            content.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, 5000);
            var commentDb = await AssertDb.EventComments.SingleAsync(x => x.Id == content.Id);
        }

        [Fact]
        public async Task GivenInvalidRequest_WhenReplyingToReplyComment_BadRequestIsReturned()
        {
            var (authUser, user) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(user);
            var comment = ArrangeDb.Add(new EventComment("bla", user, @event)).Entity;
            var replyComment = ArrangeDb.Add(new EventComment("bla reply", user, @event, comment)).Entity;
            await ArrangeDb.SaveChangesAsync();
            var request = new CreateComment.Command { Text = "New comment", ParentId = replyComment.Id };

            using var client = CreateClientForUser(authUser);
            var response = await client.PostAsJsonAsync($"/api/events/{@event.Id}/comments", request);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsAsync<ErrorResponse>();
            content.Should().BeEquivalentTo(new
            {
                ErrorCode = "ValidationError",
                ErrorDetails = new object[]
                {
                    new
                    {
                        Code = ErrorCodes.InvalidOperation,
                        PropertyName="parentId"
                    }
                }
            });
        }
    }
}
