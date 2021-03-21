using System;
using System.Collections.Generic;
using System.Linq;
using Fiesta.Domain.Common;
using Fiesta.Domain.Entities.Events;

namespace Fiesta.Domain.Entities.Users
{
    public class FiestaUser : Entity<string>
    {
        public FiestaUser(string email, string username)
        {
            Email = email;
            Username = username;
            CreatedOnUtc = DateTime.UtcNow;
        }

        public static FiestaUser CreateWithId(string id, string email, string username)
            => new FiestaUser(email, username) { Id = id };

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get => $"{FirstName} {LastName}"; }

        public string Username { get; private set; }

        public string Email { get; private set; }

        public string PictureUrl { get; set; }

        public string Bio { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOnUtc { get; init; }

        private readonly List<Event> _organizedEvents = new List<Event>();
        public IReadOnlyCollection<Event> OrganizedEvents => _organizedEvents;

        public void AddOrganizedEvent(Event organizedEvent) => _organizedEvents.Add(organizedEvent);

        public void UpdateUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("Username is null");

            Username = username;
        }

        private List<UserFriend> _friends;
        public IReadOnlyCollection<UserFriend> Friends => _friends;

        public void AddFriendRequest(FiestaUser friend)
        {
            if (_friends is null)
                _friends = new List<UserFriend>();

            _friends.Add(new UserFriend(this, friend ?? throw new ArgumentNullException(nameof(friend))));
        }

        public void AddFriend(FiestaUser friend)
        {
            //TODO: Test if entity to entity comparison works
            var foundFriend = _friends.SingleOrDefault(x => x.Friend == (friend ?? throw new ArgumentNullException(nameof(friend))));

            if (foundFriend is null)
                throw new InvalidOperationException($"Friend request not found for user IDs {friend.Id} and {Id}");

            foundFriend.ConfirmFriendRequest();
        }

        public void RemoveFriendOrFriendRequest(UserFriend friend)
        {
            _friends.Remove(friend ?? throw new ArgumentNullException(nameof(friend)));
        }
    }
}
