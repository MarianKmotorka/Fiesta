using System.Linq;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Domain.Entities.Events;
using Fiesta.Domain.Entities.Users;
using FluentAssertions;
using TestBase;
using TestBase.Assets;
using Xunit;
using static Fiesta.Application.Features.Selectors.EventsAndUsersSelector;

namespace Fiesta.WebApi.Tests.Features.Selectors
{
    // NOTE: The Dto property mappings in the Select() statement in the Handler class need to be in the same order
    //       (mapping to User and mapping to Event need to have same property mapping order)
    //       otherwise EF Core InMemory Provider fails to do the mapping.

    [Collection(nameof(TestCollection))]
    public class EventsAndUsersSelectorTests : DbTestBase
    {
        private FiestaUser _user;
        private Event _publicEvent;
        private Event _friendsOnlyEvent;
        private Event _privateEvent;
        private Handler _sut;

        public EventsAndUsersSelectorTests()
        {
            _user = ArrangeDb.SeedBasicUser().fiestaUser;
            _publicEvent = ArrangeDb.SeedEvent();
            _friendsOnlyEvent = ArrangeDb.SeedEvent(null, x => x.AccessibilityType = AccessibilityType.FriendsOnly);
            _privateEvent = ArrangeDb.SeedEvent(null, x => x.AccessibilityType = AccessibilityType.Private);

            _sut = new Handler(ActDb);

            ArrangeDb.SaveChanges();
        }

        [Fact]
        public async Task WhenQueryingData_EventsAndUsersAreReturned()
        {
            var request = new Query { CurrentUserId = _user.Id };
            var result = await _sut.Handle(request, default);

            var ids = result.Select(x => x.Id).ToList();
            ids.Should().BeEquivalentTo(new[] { _publicEvent.Id, _publicEvent.OrganizerId, _friendsOnlyEvent.OrganizerId, _privateEvent.OrganizerId, _user.Id });
        }

        [Fact]
        public async Task GivenUserHasNoRelationToEvent_WhenQueryingData_OnlyPublicEventsAreReturned()
        {
            var request = new Query { CurrentUserId = _user.Id };
            var result = await _sut.Handle(request, default);

            var eventIds = result.Where(x => x.Type == ItemType.Event).Select(x => x.Id).ToList();
            eventIds.Should().BeEquivalentTo(new[] { _publicEvent.Id });
        }

        [Fact]
        public async Task GivenUserIsInvitedToPrivateEvent_WhenQueryingData_PublicAndInvitedEventsAreReturned()
        {
            _privateEvent.AddInvitations(_user);
            await ArrangeDb.SaveChangesAsync();

            var request = new Query { CurrentUserId = _user.Id };
            var result = await _sut.Handle(request, default);

            var eventIds = result.Where(x => x.Type == ItemType.Event).Select(x => x.Id).ToList();
            eventIds.Should().BeEquivalentTo(new[] { _publicEvent.Id, _privateEvent.Id });
        }

        [Fact]
        public async Task GivenUserIsAttendeeOfPrivateEvent_WhenQueryingData_PublicAndAttendedEventsAreReturned()
        {
            _privateEvent.AddAttendee(_user);
            await ArrangeDb.SaveChangesAsync();

            var request = new Query { CurrentUserId = _user.Id };
            var result = await _sut.Handle(request, default);

            var eventIds = result.Where(x => x.Type == ItemType.Event).Select(x => x.Id).ToList();
            eventIds.Should().BeEquivalentTo(new[] { _publicEvent.Id, _privateEvent.Id });
        }

        [Fact]
        public async Task GivenUserBeingFriendOfOrganizerOfFriendsOnlyEvent_WhenQueryingData_PublicAndFriendsOnlyEventsAreReturned()
        {
            _user.AddFriend(_friendsOnlyEvent.Organizer);
            await ArrangeDb.SaveChangesAsync();

            var request = new Query { CurrentUserId = _user.Id };
            var result = await _sut.Handle(request, default);

            var eventIds = result.Where(x => x.Type == ItemType.Event).Select(x => x.Id).ToList();
            eventIds.Should().BeEquivalentTo(new[] { _publicEvent.Id, _friendsOnlyEvent.Id });
        }

        [Fact]
        public async Task GivenUserBeingFriendOfOrganizerOfPrivateEvent_WhenQueryingData_PublicEventsAreReturnedWithoutPrivateEvent()
        {
            _user.AddFriend(_privateEvent.Organizer);
            await ArrangeDb.SaveChangesAsync();

            var request = new Query { CurrentUserId = _user.Id };
            var result = await _sut.Handle(request, default);

            var eventIds = result.Where(x => x.Type == ItemType.Event).Select(x => x.Id).ToList();
            eventIds.Should().BeEquivalentTo(new[] { _publicEvent.Id });
        }

        [Fact]
        public async Task GivenUserBeingOrganizerOfPrivateEvent_WhenQueryingData_PublicAndOrganizedEventsAreReturned()
        {
            var organizedEvent = ArrangeDb.SeedEvent(_user);
            await ArrangeDb.SaveChangesAsync();

            var request = new Query { CurrentUserId = _user.Id };
            var result = await _sut.Handle(request, default);

            var eventIds = result.Where(x => x.Type == ItemType.Event).Select(x => x.Id).ToList();
            eventIds.Should().BeEquivalentTo(new[] { _publicEvent.Id, organizedEvent.Id });
        }

        [Fact]
        public async Task GivenUserBeingAdmin_WhenQueryingData_AllEventsAreReturned()
        {
            var request = new Query { CurrentUserId = _user.Id, Role = FiestaRoleEnum.Admin };
            var result = await _sut.Handle(request, default);

            var eventIds = result.Where(x => x.Type == ItemType.Event).Select(x => x.Id).ToList();
            eventIds.Should().BeEquivalentTo(new[] { _publicEvent.Id, _friendsOnlyEvent.Id, _privateEvent.Id });
        }
    }
}
