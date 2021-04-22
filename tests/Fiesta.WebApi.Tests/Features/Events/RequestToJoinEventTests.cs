using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.WebApi.Middleware.ExceptionHanlding;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TestBase.Assets;
using Xunit;

namespace Fiesta.WebApi.Tests.Features.Events
{
    [Collection(nameof(TestCollection))]
    public class RequestToJoinEventTests : WebAppTestBase
    {
        public RequestToJoinEventTests(FiestaAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenValidRequest_WhenRequestingToJoin_RequestIsCreated()
        {
            var (_, organizer) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/join-requests", new { });
            response.EnsureSuccessStatusCode();

            var eventDb = await AssertDb.Events.Include(x => x.JoinRequests).SingleAsync(x => x.Id == @event.Id);
            eventDb.JoinRequests.Should().BeEquivalentTo(new[]
            {
                new{ InterestedUserId=LoggedInUserId, EventId=@event.Id },
            });
        }

        [Fact]
        public async Task GivenUserAlreadyAttendee_WhenRequestingToJoin_BadRequestIsReturned()
        {
            var attendee = await ArrangeDb.FiestaUsers.FindAsync(LoggedInUserId);
            var (_, organizer) = ArrangeDb.SeedBasicUser();
            var @event = ArrangeDb.SeedEvent(organizer);
            @event.AddAttendee(attendee);
            await ArrangeDb.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync($"/api/events/{@event.Id}/join-requests", new { });
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
                        PropertyName="eventId"
                    }
                }
            });
        }
    }
}

