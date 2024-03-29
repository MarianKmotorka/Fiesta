﻿using System.Net;
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
    public class InviteUsersToEventTests : WebAppTestBase
    {
        public InviteUsersToEventTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenValidRequest_WhenInvitingUsers_UsersAreInvited()
        {
            var organizer = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);
            var (_, user1) = ArrangeDb.SeedBasicUser();
            var (_, user2) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/invitations", new { invitedIds = new[] { user1.Id, user2.Id } });
            response.EnsureSuccessStatusCode();

            var eventDb = await AssertDb.Events.Include(x => x.Invitations).SingleAsync(x => x.Id == @event.Id);
            eventDb.Invitations.Should().BeEquivalentTo(new[]
            {
                new{ InviterId = organizer.Id, InviteeId = user1.Id, EventId=@event.Id },
                new{ InviterId = organizer.Id, InviteeId = user2.Id, EventId=@event.Id },
            });

            var notifications = await AssertDb.Notifications.ToListAsync();
            notifications.Should().HaveCount(2);
            notifications.Should().BeEquivalentTo(new object[] { new { UserId = user1.Id }, new { UserId = user2.Id } });
            notifications.ForEach(n =>
            {
                n.GetModel<EventInvitationCreatedNotification>().Should().BeEquivalentTo(
                    new EventInvitationCreatedNotification
                    {
                        EventId = @event.Id,
                        EventName = @event.Name,
                        InviterId = @event.Organizer.Id,
                        InviterUsername = @event.Organizer.Username
                    }
                );
            });
        }

        [Fact]
        public async Task GivenInvitedIdsContainingAlreadyInvitedUser_WhenInvitingUsers_BadRequestIsReturned()
        {
            var organizer = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);
            var (_, user1) = ArrangeDb.SeedBasicUser();
            var (_, user2) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer);
            @event.AddInvitations(user1);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/invitations", new { invitedIds = new[] { user1.Id, user2.Id } });
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var errorResposne = await response.Content.ReadAsAsync<ErrorResponse>();
            errorResposne.Should().BeEquivalentTo(new
            {
                ErrorCode = "ValidationError",
                ErrorDetails = new object[]
                {
                    new
                    {
                        Code = ErrorCodes.AlreadyAttendeeOrInvited,
                        PropertyName="invitedIds"
                    }
                }
            });
        }

        [Fact]
        public async Task GivenInvitedIdsContainingAttendee_WhenInvitingUsers_BadRequestIsReturned()
        {
            var organizer = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);
            var (_, user1) = ArrangeDb.SeedBasicUser();
            var (_, user2) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer);
            @event.AddAttendee(user1);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/invitations", new { invitedIds = new[] { user1.Id, user2.Id } });
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var errorResposne = await response.Content.ReadAsAsync<ErrorResponse>();
            errorResposne.Should().BeEquivalentTo(new
            {
                ErrorCode = "ValidationError",
                ErrorDetails = new object[]
                {
                    new
                    {
                        Code = ErrorCodes.AlreadyAttendeeOrInvited,
                        PropertyName="invitedIds"
                    }
                }
            });
        }

        [Fact]
        public async Task GivenInvitedIdsContainingOrganizer_WhenInvitingUsers_BadRequestIsReturned()
        {
            var organizer = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);
            var @event = ArrangeDb.SeedEvent(organizer);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/invitations", new { invitedIds = new[] { organizer.Id } });
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var errorResposne = await response.Content.ReadAsAsync<ErrorResponse>();
            errorResposne.Should().BeEquivalentTo(new
            {
                ErrorCode = "ValidationError",
                ErrorDetails = new object[]
                {
                    new
                    {
                        Code = ErrorCodes.AlreadyAttendeeOrInvited,
                        PropertyName="invitedIds"
                    }
                }
            });
        }

        [Fact]
        public async Task GivenUserWhoIsNotOrganizer_WhenInvitingUsers_ForbiddenIsReturned()
        {
            var (_, organizer) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/invitations", new { invitedIds = new[] { "some_id" } });
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
