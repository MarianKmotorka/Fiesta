using System;
using System.Collections.Generic;
using Fiesta.Domain.Common;
using Fiesta.Domain.Entities.Users;

namespace Fiesta.Domain.Entities.Events
{
    public class EventComment : Entity<string>
    {
        private List<EventComment> _replies;

        public EventComment(string text, FiestaUser sender, Event @event, EventComment parent = null)
        {
            CreatedAt = DateTime.UtcNow;
            Text = text.Trim();

            Sender = sender;
            SenderId = sender.Id;

            Event = @event;
            EventId = @event.Id;

            Parent = parent;
            ParentId = parent?.Id;
        }

        private EventComment()
        {
        }

        public string Text { get; private set; }

        public string SenderId { get; private set; }

        public FiestaUser Sender { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public string ParentId { get; private set; }

        public EventComment Parent { get; private set; }

        public string EventId { get; private set; }

        public Event Event { get; private set; }

        public bool IsEdited { get; private set; }

        public IReadOnlyCollection<EventComment> Replies => _replies;

        public void Edit(string text)
        {
            Text = text;
            IsEdited = true;
        }
    }
}
