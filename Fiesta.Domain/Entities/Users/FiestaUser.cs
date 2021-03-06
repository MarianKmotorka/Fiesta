using Fiesta.Domain.Common;
using Fiesta.Domain.Entities.Events;
using System;
using System.Collections.Generic;

namespace Fiesta.Domain.Entities.Users
{
    public class FiestaUser : Entity<string>
    {
        public FiestaUser(string email)
        {
            Email = email;
            CreatedOnUtc = DateTime.UtcNow;
        }

        public static FiestaUser CreateWithId(string id, string email)
            => new FiestaUser(email) { Id = id };

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; private set; }

        public string PictureUrl { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOnUtc { get; init; }

        private readonly List<Event> _organizedEvents = new List<Event>();
        public IReadOnlyCollection<Event> OrganizedEvents => _organizedEvents;

        public void AddOrganizedEvent(Event organizedEvent)
        {
            _organizedEvents.Add(organizedEvent);
        }
    }
}
