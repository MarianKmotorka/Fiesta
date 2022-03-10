using System.Linq;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Domain.Entities.Events;
using Fiesta.Domain.Entities.Users;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase;
using TestBase.Assets;
using static Fiesta.Application.Features.Users.GetUserAttendedEvents;

namespace Fiesta.WebApi.Tests.Features.Users
{
    [TestClass]
    public class GetUserAttendedEventsTests : DbTestBase
    {
        private FiestaUser _organizer;
        private FiestaUser _attendee;
        private FiestaUser _currentUser;
        private Event _publicEvent;
        private Event _friendsOnlyEventByOrganizer;
        private Event _privateEventByOrganizer;
        private Event _friendsOnlyEvent;
        private Event _privateEvent;
        private Handler _sut;

        [TestInitialize]
        public void Init()
        {
            _organizer = ArrangeDb.SeedBasicUser().fiestaUser;
            _attendee = ArrangeDb.SeedBasicUser().fiestaUser;
            _currentUser = ArrangeDb.SeedBasicUser().fiestaUser;
            _publicEvent = ArrangeDb.SeedEvent();
            var notAttendedEvent = ArrangeDb.SeedEvent();
            _friendsOnlyEvent = ArrangeDb.SeedEvent(null, x => x.AccessibilityType = AccessibilityType.FriendsOnly);
            _privateEvent = ArrangeDb.SeedEvent(null, x => x.AccessibilityType = AccessibilityType.Private);
            _friendsOnlyEventByOrganizer = ArrangeDb.SeedEvent(_organizer, x => x.AccessibilityType = AccessibilityType.FriendsOnly);
            _privateEventByOrganizer = ArrangeDb.SeedEvent(_organizer, x => x.AccessibilityType = AccessibilityType.Private);

            _publicEvent.AddAttendee(_attendee);
            _friendsOnlyEvent.AddAttendee(_attendee);
            _privateEvent.AddAttendee(_attendee);
            _friendsOnlyEventByOrganizer.AddAttendee(_attendee);
            _privateEventByOrganizer.AddAttendee(_attendee);

            _sut = new Handler(ActDb);
            ArrangeDb.SaveChanges();
        }

        [TestMethod]
        public async Task GivenUserWithNoRelationToEventOrAttendeeOrEvents_WhenUserRequestsAttendedEvents_OnlyPublicEventsAreReturned()
        {
            var query = new Query { CurrentUserId = _currentUser.Id, UserId = _attendee.Id };
            var result = (await _sut.Handle(query, default)).Entries;
            var returnedEvent = result.Single();
            returnedEvent.Id.Should().Be(_publicEvent.Id);
        }

        [TestMethod]
        public async Task GivenUserBeingFriendOfOrganizer_WhenUserRequestsAttendedEvents_AlsoFriendsOnlyEventsAreReturned()
        {
            _organizer.AddFriend(_currentUser);
            await ArrangeDb.SaveChangesAsync();

            var query = new Query { CurrentUserId = _currentUser.Id, UserId = _attendee.Id };
            var result = (await _sut.Handle(query, default)).Entries.Select(x => x.Id);

            result.Should().BeEquivalentTo(new[] { _publicEvent.Id, _friendsOnlyEventByOrganizer.Id });
        }

        [TestMethod]
        public async Task GivenUserBeingAlsoAttendee_WhenUserRequestsAttendedEvents_AlsoEventsWhereUserIsAttendeeAreReturned()
        {
            _privateEventByOrganizer.AddAttendee(_currentUser);
            _friendsOnlyEvent.AddAttendee(_currentUser);
            await ArrangeDb.SaveChangesAsync();

            var query = new Query { CurrentUserId = _currentUser.Id, UserId = _attendee.Id };
            var result = (await _sut.Handle(query, default)).Entries.Select(x => x.Id);

            result.Should().BeEquivalentTo(new[] { _publicEvent.Id, _friendsOnlyEvent.Id, _privateEventByOrganizer.Id });
        }

        [TestMethod]
        public async Task GivenUserBeingInvited_WhenUserRequestsAttendedEvents_AlsoEventsWhereUserIsInvitedAreReturned()
        {
            _privateEvent.AddInvitations(_currentUser);
            _friendsOnlyEventByOrganizer.AddInvitations(_currentUser);
            await ArrangeDb.SaveChangesAsync();

            var query = new Query { CurrentUserId = _currentUser.Id, UserId = _attendee.Id };
            var result = (await _sut.Handle(query, default)).Entries.Select(x => x.Id);

            result.Should().BeEquivalentTo(new[] { _publicEvent.Id, _friendsOnlyEventByOrganizer.Id, _privateEvent.Id });
        }

        [TestMethod]
        public async Task GivenUserAdmin_WhenUserRequestsAttendedEvents_AllEventsAreReturned()
        {
            var query = new Query { CurrentUserId = _currentUser.Id, UserId = _attendee.Id, Role = FiestaRoleEnum.Admin };
            var result = (await _sut.Handle(query, default)).Entries.Select(x => x.Id);
            result.Should().BeEquivalentTo(new[] { _publicEvent.Id, _friendsOnlyEvent.Id, _friendsOnlyEventByOrganizer.Id, _privateEvent.Id, _privateEventByOrganizer.Id });
        }

        [TestMethod]
        public async Task WhenUserRequestsHisOwnAttendedEvents_AllEventsAreReturned()
        {
            var query = new Query { CurrentUserId = _attendee.Id, UserId = _attendee.Id };
            var result = (await _sut.Handle(query, default)).Entries.Select(x => x.Id);

            result.Should().BeEquivalentTo(new[] { _publicEvent.Id, _friendsOnlyEvent.Id, _friendsOnlyEventByOrganizer.Id, _privateEvent.Id, _privateEventByOrganizer.Id });
        }

        [TestMethod]
        public async Task GivenUserBeingOrganizer_WhenUserRequestsAttendedEvents_AlsoEventsWhereUserIsOrganizerAreReturned()
        {
            var query = new Query { CurrentUserId = _organizer.Id, UserId = _attendee.Id };
            var result = (await _sut.Handle(query, default)).Entries.Select(x => x.Id);

            result.Should().BeEquivalentTo(new[] { _publicEvent.Id, _friendsOnlyEventByOrganizer.Id, _privateEventByOrganizer.Id });
        }
    }
}
