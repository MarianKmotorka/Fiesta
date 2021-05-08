using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Models.Notifications;
using Fiesta.WebApi.Middleware.ExceptionHanlding;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TestBase.Assets;
using Xunit;


namespace Fiesta.WebApi.Tests.Features.Events
{
    [Collection(nameof(TestCollection))]
    public class ReplyToEventInvitationTests : WebAppTestBase
    {
        public ReplyToEventInvitationTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GiveExistingInvitation_WhenAcceptingTheInvitation_EventAttendeeIsAddedAndInvitationIsRemoved()
        {
            var invitee = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);
            var (_, organizer) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer);
            @event.AddInvitations(invitee);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/invitations/reply", new { Accepted = true });
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

            var notification = await AssertDb.Notifications.SingleAsync();
            notification.Seen.Should().BeFalse();
            notification.UserId.Should().Be(organizer.Id);
            notification.GetModel<EventInvitationReplyNotification>().Should().BeEquivalentTo(new
            {
                EventId = @event.Id,
                EventName = @event.Name,
                Accepted = true,
                InvitedUsername = invitee.Username,
                InvitedId = invitee.Id
            });
        }

        [Fact]
        public async Task GiveExistingInvitation_WhenRejectingTheInvitation_InvitationIsRemoved()
        {
            var invitee = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);
            var (_, organizer) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer);
            @event.AddInvitations(invitee);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/invitations/reply", new { Accepted = false });
            response.EnsureSuccessStatusCode();

            var eventDb = await AssertDb.Events
                .Include(x => x.Invitations)
                .Include(x => x.Attendees)
                .SingleAsync(x => x.Id == @event.Id);

            eventDb.Invitations.Should().BeEmpty();
            eventDb.Attendees.Should().BeEmpty();

            var notification = await AssertDb.Notifications.SingleAsync();
            notification.Seen.Should().BeFalse();
            notification.UserId.Should().Be(organizer.Id);
            notification.GetModel<EventInvitationReplyNotification>().Should().BeEquivalentTo(new
            {
                EventId = @event.Id,
                EventName = @event.Name,
                Accepted = false,
                InvitedUsername = invitee.Username,
                InvitedId = invitee.Id
            });
        }

        [Fact]
        public async Task GiveExistingInvitationOnFullEvent_WhenAcceptingTheInvitation_BadRequestIsReturned()
        {
            var invitee = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);
            var (_, attendee) = ArrangeDb.SeedBasicUser();
            var (_, organizer) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer, x => x.Capacity = 1);
            @event.AddInvitations(invitee);
            @event.AddAttendee(attendee);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/invitations/reply", new { Accepted = true });
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var error = await response.Content.ReadAsAsync<ErrorResponse>();
            error.Should().BeEquivalentTo(new
            {
                ErrorCode = "ValidationError",
                ErrorDetails = new object[]
                {
                    new
                    {
                        Code = ErrorCodes.EventIsFull
                    }
                }
            });
        }
    }
}
