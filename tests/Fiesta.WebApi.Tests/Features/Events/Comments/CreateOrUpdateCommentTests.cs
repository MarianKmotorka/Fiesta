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
using TestBase.Helpers;
using Xunit;

namespace Fiesta.WebApi.Tests.Features.Events.Comments
{
    [Collection(nameof(TestCollection))]
    public class CreateOrUpdateCommentTests : WebAppTestBase
    {
        public CreateOrUpdateCommentTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenValidRequest_WhenCreatingComment_CommentIsCreated()
        {
            var (authUser, user) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(user);
            await ArrangeDb.SaveChangesAsync();
            var request = new CreateOrUpdateComment.Command { Text = "New comment" };

            using var client = CreateClientForUser(authUser);
            var response = await client.PostAsJsonAsync($"/api/events/{@event.Id}/comments", request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsAsync<CommentDto>();
            content.Should().BeEquivalentTo(new
            {
                Text = request.Text,
                IsEdited = false,
                ParentId = default(string),
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
            var request = new CreateOrUpdateComment.Command { Text = "New comment", ParentId = replyComment.Id };

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

        [Fact]
        public async Task GivenValidRequest_WhenUpdatingComment_CommentIsUpdated()
        {
            var (authUser, user) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(user);
            var comment = ArrangeDb.Add(new EventComment("Comment", user, @event)).Entity;
            await ArrangeDb.SaveChangesAsync();
            var request = new CreateOrUpdateComment.Command { Text = "Updated comment" };

            using var client = CreateClientForUser(authUser);
            var response = await client.PatchAsJsonAsync($"/api/events/{@event.Id}/comments/{comment.Id}", request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsAsync<CommentDto>();
            content.Should().BeEquivalentTo(new
            {
                Text = request.Text,
                IsEdited = true,
                ParentId = default(string),
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
        }

        [Fact]
        public async Task GivenExistingComment_WhenEditingCommentOfSomeoneElse_ForbiddenIsReturned()
        {
            var (_, attendee) = ArrangeDb.SeedBasicUser();
            var (authUser, otherAttendee) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent();
            @event.AddAttendee(attendee);
            @event.AddAttendee(otherAttendee);

            var comment = ArrangeDb.Add(new EventComment("bla", attendee, @event)).Entity;
            await ArrangeDb.SaveChangesAsync();
            var request = new CreateOrUpdateComment.Command { Text = "Updated comment" };

            using var client = CreateClientForUser(authUser);
            var response = await client.PatchAsJsonAsync($"/api/events/{@event.Id}/comments/{comment.Id}", request);
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
