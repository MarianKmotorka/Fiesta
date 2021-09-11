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

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public AccessibilityType AccessibilityType { get; set; }

        public int Capacity { get; set; }

        public string Description { get; set; }

        public LocationObject Location { get; private set; }

        public string ExternalLink { get; private set; }

        public FiestaUser Organizer { get; private set; }

        public string OrganizerId { get; private set; }

        public string BannerUrl { get; set; }

        public IReadOnlyCollection<EventAttendee> Attendees => _attendees;

        public IReadOnlyCollection<EventInvitation> Invitations => _invitations;

        public IReadOnlyCollection<EventJoinRequest> JoinRequests => _joinRequests;

        public Event(string name, DateTime startDate, DateTime endDate, AccessibilityType accessibilityType, int capacity, FiestaUser organizer, LocationObject location)
        : this(name, startDate, endDate, accessibilityType, capacity, organizer)
        {
            Location = location;
        }

        public Event(string name, DateTime startDate, DateTime endDate, AccessibilityType accessibilityType, int capacity, FiestaUser organizer, string externalLink)
        : this(name, startDate, endDate, accessibilityType, capacity, organizer)
        {
            ExternalLink = externalLink;
        }

        private Event(string name, DateTime startDate, DateTime endDate, AccessibilityType accessibilityType, int capacity, FiestaUser organizer)
        {
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            AccessibilityType = accessibilityType;
            Capacity = capacity;
            Organizer = organizer;
            OrganizerId = organizer.Id;
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

        public void AddInvitations(params FiestaUser[] invitees)
        {
            if (_invitations is null)
                _invitations = new();

            if (Organizer is null)
                throw new Exception("Event.Organizer entity not loaded.");

            foreach (var invitee in invitees)
                _invitations.Add(new EventInvitation(this, Organizer, invitee));
        }

        public EventJoinRequest AddJoinRequest(FiestaUser interestedUser)
        {
            if (_joinRequests is null)
                _joinRequests = new();

            var joinRequest = new EventJoinRequest(this, interestedUser);

            _joinRequests.Add(joinRequest);

            return joinRequest;
        }

        public void SetDescription(string description)
        {
            Description = description?.Replace(Environment.NewLine, "").Trim();
        }

        public void PublishDeletedEvent()
        {
            AddDomainEvent(new EventDeletedEvent(this));
        }

        public void SetLocation(LocationObject location)
        {
            Location = location ?? throw new ArgumentNullException(nameof(location));
            ExternalLink = null;
        }

        public void SetExternalLink(string link)
        {
            ExternalLink = string.IsNullOrEmpty(link)
                ? throw new ArgumentException("Parameter link cannot be null or empty")
                : link;

            Location = null;
        }
    }

    public enum AccessibilityType
    {
        Public,
        Private,
        FriendsOnly
    }
}
