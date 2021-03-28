using System;
using System.Collections.Generic;
using System.Linq;
using Fiesta.Domain.Common;
using Fiesta.Domain.Entities.Users;

namespace Fiesta.Domain.Entities.Events
{
    public class Event : Entity<string>
    {
        private List<EventAttendee> _attendees;
        private List<EventInvitation> _invitations;
        private List<EventJoinRequest> _joinRequests;

        public string Name { get; private set; }

        public DateTime StartDate { get; private set; }

        public DateTime EndDate { get; private set; }

        public AccessibilityType AccessibilityType { get; private set; }

        public int Capacity { get; private set; }

        public string Description { get; set; }

        public LocationObject Location { get; private set; }

        public FiestaUser Organizer { get; private set; }

        public IReadOnlyCollection<EventAttendee> Attendees => _attendees;

        public IReadOnlyCollection<EventInvitation> Invitations => _invitations;

        public IReadOnlyCollection<EventJoinRequest> JoinRequests => _joinRequests;

        public Event(string name, DateTime startDate, DateTime endDate, AccessibilityType accessibilityType, int capacity, FiestaUser organizer, LocationObject location)
        {
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            AccessibilityType = accessibilityType;
            Capacity = capacity;
            Organizer = organizer;
            Location = location;
        }

        private Event()
        {
        }

        public void AddAttendee(FiestaUser attendee)
        {
            if (_attendees is null)
                _attendees = new();

            _attendees.Add(new EventAttendee(attendee, this));
        }

        public void RemoveAttendee(FiestaUser attendee)
        {
            var toBeRemoved = _attendees.Single(x => x.Attendee == attendee);
            _attendees.Remove(toBeRemoved);
        }

        public void AddInvitation(FiestaUser inviter, FiestaUser invitee)
        {
            if (_invitations is null)
                _invitations = new();

            _invitations.Add(new EventInvitation(this, inviter, invitee));
        }

        public void AddJoinRequest(FiestaUser interestedUser)
        {
            if (_joinRequests is null)
                _joinRequests = new();

            _joinRequests.Add(new EventJoinRequest(this, interestedUser));
        }
    }

    public enum AccessibilityType
    {
        Public,
        Private,
        FriendsOnly
    }
}
