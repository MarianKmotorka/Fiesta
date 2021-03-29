using System;
using System.Collections.Generic;
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

        public AccessibilityType AccessibilityType { get; set; }

        public int Capacity { get; private set; }

        public string Description { get; set; }

        public LocationObject Location { get; private set; }

        public FiestaUser Organizer { get; private set; }

        public string OrganizerId { get; private set; }

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
            OrganizerId = organizer.Id;
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

        public void AddInvitation(FiestaUser invitee)
        {
            if (_invitations is null)
                _invitations = new();

            if (Organizer is null)
                throw new Exception("Event.Organizor entity not loaded.");

            _invitations.Add(new EventInvitation(this, Organizer, invitee));
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
