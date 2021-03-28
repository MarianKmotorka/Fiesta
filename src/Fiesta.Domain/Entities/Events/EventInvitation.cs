using Fiesta.Domain.Entities.Users;

namespace Fiesta.Domain.Entities.Events
{
    public class EventInvitation
    {
        public Event Event { get; init; }

        public FiestaUser Inviter { get; init; }

        public FiestaUser Invitee { get; init; }

        public string EventId { get; init; }

        public string InviterId { get; init; }

        public string InviteeId { get; init; }

        public EventInvitation(Event @event, FiestaUser inviter, FiestaUser invitee)
        {
            Event = @event;
            Inviter = inviter;
            Invitee = invitee;
            EventId = @event.Id;
            InviterId = inviter.Id;
            InviteeId = invitee.Id;
        }

        private EventInvitation()
        {
        }
    }
}
