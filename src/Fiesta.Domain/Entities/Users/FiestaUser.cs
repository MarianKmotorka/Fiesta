using System;
using System.Collections.Generic;
using System.Linq;
using Fiesta.Domain.Common;
using Fiesta.Domain.Entities.Events;

namespace Fiesta.Domain.Entities.Users
{
    public class FiestaUser : Entity<string>
    {
        private List<Event> _organizedEvents;
        private List<UserFriend> _friends;
        private List<FriendRequest> _recievedFriendRequests;
        private List<FriendRequest> _sentFriendRequests;
        private List<EventAttendee> _attendedEvents;
        private List<EventInvitation> _recievedEventInvitations;
        private List<EventInvitation> _sentEventInvitations;
        private List<EventJoinRequest> _sentEventJoinRequests;

        private FiestaUser()
        {
        }

        public FiestaUser(string email, string username)
        {
            Email = email;
            Username = username;
            CreatedOnUtc = DateTime.UtcNow;
            _organizedEvents = new();
            _friends = new();
            _recievedFriendRequests = new();
            _sentFriendRequests = new();
            _attendedEvents = new();
            _recievedEventInvitations = new();
            _sentEventInvitations = new();
            _sentEventJoinRequests = new();
        }

        public static FiestaUser CreateWithId(string id, string email, string username)
            => new(email, username) { Id = id };

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get => $"{FirstName} {LastName}"; }

        public string Username { get; private set; }

        public string Email { get; private set; }

        public string PictureUrl { get; set; }

        public string Bio { get; private set; }

        public bool IsDeleted { get; private set; }

        public DateTime CreatedOnUtc { get; init; }

        public IReadOnlyCollection<Event> OrganizedEvents => _organizedEvents;

        public IReadOnlyCollection<UserFriend> Friends => _friends;

        public IReadOnlyCollection<FriendRequest> RecievedFriendRequests => _recievedFriendRequests;

        public IReadOnlyCollection<FriendRequest> SentFriendRequests => _sentFriendRequests;

        public IReadOnlyCollection<EventAttendee> AttendedEvents => _attendedEvents;

        /// <summary>
        /// Events that user have been invited to.
        /// </summary>
        public IReadOnlyCollection<EventInvitation> RecievedEventInvitations => _recievedEventInvitations;

        /// <summary>
        /// Events that user (event organizator) has invited other users to.
        /// </summary>
        public IReadOnlyCollection<EventInvitation> SentEventInvitations => _sentEventInvitations;

        /// <summary>
        /// Events that user requested to join.
        /// </summary>
        public IReadOnlyCollection<EventJoinRequest> SentEventJoinRequests => _sentEventJoinRequests;

        public void UpdateUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException(nameof(username));

            Username = username;
        }

        public void SendFriendRequest(FiestaUser friend)
        {
            if (_sentFriendRequests is null)
                _sentFriendRequests = new List<FriendRequest>();

            _ = friend ?? throw new ArgumentNullException(nameof(friend));
            _sentFriendRequests.Add(new FriendRequest(this, friend));
        }

        public void AddFriend(FiestaUser friend)
        {
            if (_friends is null)
                _friends = new List<UserFriend>();

            _ = friend ?? throw new ArgumentNullException(nameof(friend));

            var friendRelation = _friends.SingleOrDefault(x => x.FriendId == friend.Id);

            if (friendRelation is not null)
                return;

            _friends.Add(new UserFriend(this, friend));
            friend.AddFriend(this);
        }

        public void RemoveFriend(FiestaUser friend)
        {
            _ = friend ?? throw new ArgumentNullException(nameof(friend));
            var friendRelation = _friends.SingleOrDefault(x => x.FriendId == friend.Id);

            if (friendRelation is null)
                return;

            _friends.Remove(friendRelation);
            friend.RemoveFriend(this);
        }

        public void SetBio(string bio)
        {
            Bio = bio.Replace(Environment.NewLine, "").Trim();
        }

        public void SetDeleted()
        {
            if (IsDeleted)
                return;

            IsDeleted = true;
            AddDomainEvent(new FiestaUserDeletedEvent(this));

            if (OrganizedEvents is null)
                throw new Exception("FiestaUser.OrganizedEvents is not loaded.");

            foreach (var @event in OrganizedEvents)
                @event.PublishDeletedEvent();
        }
    }
}
