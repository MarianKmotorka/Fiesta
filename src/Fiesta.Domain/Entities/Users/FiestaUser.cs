﻿using System;
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

        public IReadOnlyCollection<Event> OrganizedEvents => _organizedEvents;

        public IReadOnlyCollection<UserFriend> Friends => _friends;

        public IReadOnlyCollection<FriendRequest> RecievedFriendRequests => _recievedFriendRequests;

        public IReadOnlyCollection<FriendRequest> SentFriendRequests => _sentFriendRequests;

        public IReadOnlyCollection<EventAttendee> AttendedEvents => _attendedEvents;

        public IReadOnlyCollection<EventInvitation> RecievedEventInvitations => _recievedEventInvitations;

        public IReadOnlyCollection<EventInvitation> SentEventInvitations => _sentEventInvitations;


        public void AddOrganizedEvent(Event organizedEvent)
        {
            if (_organizedEvents is null)
                _organizedEvents = new List<Event>();

            _organizedEvents.Add(organizedEvent ?? throw new ArgumentNullException(nameof(organizedEvent)));
        }

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
    }
}
