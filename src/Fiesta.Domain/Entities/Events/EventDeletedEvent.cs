using System;
using MediatR;

namespace Fiesta.Domain.Entities.Events
{
    public class EventDeletedEvent : INotification
    {
        public EventDeletedEvent(Event deletedEvent)
        {
            Event = deletedEvent ?? throw new ArgumentNullException(nameof(deletedEvent));
        }

        public Event Event { get; }
    }
}
