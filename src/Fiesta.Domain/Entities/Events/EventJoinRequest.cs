using Fiesta.Domain.Entities.Users;

namespace Fiesta.Domain.Entities.Events
{
    public class EventJoinRequest
    {
        public EventJoinRequest(Event @event, FiestaUser interestedUser)
        {
            Event = @event;
            InterestedUser = interestedUser;
            EventId = @event.Id;
            InterestedUserId = interestedUser.Id;
        }

        private EventJoinRequest()
        {
        }

        public Event Event { get; init; }

        public FiestaUser InterestedUser { get; init; }

        public string EventId { get; init; }

        public string InterestedUserId { get; init; }
    }
}
