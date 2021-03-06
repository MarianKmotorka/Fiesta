using Fiesta.Domain.Common;
using Fiesta.Domain.Entities.Users;
using System;

namespace Fiesta.Domain.Entities.Events
{
    public class Event : Entity<string>
    {
        public string Name { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public AccessibilityType AccessibilityType { get; private set; }
        public int Capacity { get; private set; }
        public LocationObject Location { get; private set; }
        public FiestaUser Organizer { get; private set; }

        public Event(string name, DateTime startDate, DateTime endDate, AccessibilityType accessibilityType, int capacity, FiestaUser organizer)
        {
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            AccessibilityType = accessibilityType;
            Capacity = capacity;
            Organizer = organizer;
        }

        private Event()
        {
        }
    }

    public enum AccessibilityType
    {
        Public,
        Private,
        FriendsOnly
    }
}
