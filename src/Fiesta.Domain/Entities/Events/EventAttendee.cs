using Fiesta.Domain.Entities.Users;

namespace Fiesta.Domain.Entities.Events
{
    public class EventAttendee
    {
        public FiestaUser Attendee { get; init; }

        public Event Event { get; init; }

        public string AttendeeId { get; init; }

        public string EventId { get; init; }

        public EventAttendee(FiestaUser attendee, Event @event)
        {
            Attendee = attendee;
            Event = @event;
            AttendeeId = attendee.Id;
            EventId = @event.Id;

        }

        private EventAttendee()
        {
        }
    }
}
