using System.Linq;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Domain.Entities.Events;
using Fiesta.Domain.Entities.Users;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase;
using TestBase.Assets;
using static Fiesta.Application.Features.Users.GetUserOrganizedEvents;

namespace Fiesta.WebApi.Tests.Features.Users
{
    [TestClass]
    public class GetUserOrganizedEventsTests : DbTestBase
    {
        private FiestaUser _organizer;
        private FiestaUser _currentUser;
        private Event _publicEvent;
        private Event _friendsOnlyEvent;
        private Event _privateEvent;
        private Handler _sut;

        [TestInitialize]
        public void Init()
        {
            _organizer = ArrangeDb.SeedBasicUser().fiestaUser;
            _currentUser = ArrangeDb.SeedBasicUser().fiestaUser;
            _publicEvent = ArrangeDb.SeedEvent(_organizer);
            _friendsOnlyEvent = ArrangeDb.SeedEvent(_organizer, x => x.AccessibilityType = AccessibilityType.FriendsOnly);
            _privateEvent = ArrangeDb.SeedEvent(_organizer, x => x.AccessibilityType = AccessibilityType.Private);
            var someEventNotOrganizedByOrganizer = ArrangeDb.SeedEvent();
            _sut = new Handler(ActDb);
            ArrangeDb.SaveChanges();
        }

        [TestMethod]
        public async Task GiveUserWithNoRelationToEventOrOrganizer_WhenUserRequestsOrganizedEvents_OnlyPublicEventsAreReturned()
        {
            var query = new Query { CurrentUserId = _currentUser.Id, UserId = _organizer.Id };
            var result = (await _sut.Handle(query, default)).Entries;
            var returnedEvent = result.Single();
            returnedEvent.Id.Should().Be(_publicEvent.Id);
        }

        [TestMethod]
        public async Task GiveUserInvitedToPrivateEvent_WhenUserRequestsOrganizedEvents_AlsoPrivateEventIsReturned()
        {
            _privateEvent.AddInvitations(_currentUser);
            await ArrangeDb.SaveChangesAsync();

            var query = new Query { CurrentUserId = _currentUser.Id, UserId = _organizer.Id };
            var result = (await _sut.Handle(query, default)).Entries.Select(x => x.Id);

            result.Should().BeEquivalentTo(new[] { _publicEvent.Id, _privateEvent.Id });
        }

        [TestMethod]
        public async Task GiveUserBeingAttendeeOfPrivateEvent_WhenUserRequestsOrganizedEvents_AlsoPrivateEventIsReturned()
        {
            _privateEvent.AddAttendee(_currentUser);
            await ArrangeDb.SaveChangesAsync();

            var query = new Query { CurrentUserId = _currentUser.Id, UserId = _organizer.Id };
            var result = (await _sut.Handle(query, default)).Entries.Select(x => x.Id);

            result.Should().BeEquivalentTo(new[] { _publicEvent.Id, _privateEvent.Id });
        }

        [TestMethod]
        public async Task GiveUserBeingFriendOfOrganizer_WhenUserRequestsOrganizedEvents_AlsoFriendsOnlyEventIsReturned()
        {
            _organizer.AddFriend(_currentUser);
            await ArrangeDb.SaveChangesAsync();

            var query = new Query { CurrentUserId = _currentUser.Id, UserId = _organizer.Id };
            var result = (await _sut.Handle(query, default)).Entries.Select(x => x.Id);

            result.Should().BeEquivalentTo(new[] { _publicEvent.Id, _friendsOnlyEvent.Id });
        }

        [TestMethod]
        public async Task GiveUserBeingAttendeeToEvents_WhenUserRequestsOrganizedEvents_AlsoEventsWhereUserIsAttendeeAreReturned()
        {
            _publicEvent.AddAttendee(_currentUser);
            _friendsOnlyEvent.AddAttendee(_currentUser);
            _privateEvent.AddAttendee(_currentUser);
            await ArrangeDb.SaveChangesAsync();

            var query = new Query { CurrentUserId = _currentUser.Id, UserId = _organizer.Id };
            var result = (await _sut.Handle(query, default)).Entries.Select(x => x.Id);

            result.Should().BeEquivalentTo(new[] { _publicEvent.Id, _friendsOnlyEvent.Id, _privateEvent.Id });
        }

        [TestMethod]
        public async Task GiveUserBeingAdmin_WhenUserRequestsOrganizedEvents_AllAreReturned()
        {
            var query = new Query { CurrentUserId = _currentUser.Id, UserId = _organizer.Id, Role = FiestaRoleEnum.Admin };
            var result = (await _sut.Handle(query, default)).Entries.Select(x => x.Id);

            result.Should().BeEquivalentTo(new[] { _publicEvent.Id, _friendsOnlyEvent.Id, _privateEvent.Id });
        }

        [TestMethod]
        public async Task WhenOrganizerRequestsOwnOrganizedEvents_AllAreReturned()
        {
            var query = new Query { CurrentUserId = _organizer.Id, UserId = _organizer.Id };
            var result = (await _sut.Handle(query, default)).Entries.Select(x => x.Id);

            result.Should().BeEquivalentTo(new[] { _publicEvent.Id, _friendsOnlyEvent.Id, _privateEvent.Id });
        }
    }
}
