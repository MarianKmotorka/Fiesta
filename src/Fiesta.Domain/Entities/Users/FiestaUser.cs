using Fiesta.Domain.Common;
using Fiesta.Domain.Entities.Events;
using System;
using System.Collections.Generic;

namespace Fiesta.Domain.Entities.Users
{
    public class FiestaUser : Entity<string>
    {
        public FiestaUser(string email, string username)
        {
            Email = email;
            Username = username;
            CreatedOnUtc = DateTime.UtcNow;
        }

        public static FiestaUser CreateWithId(string id, string email, string username)
            => new FiestaUser(email, username) { Id = id };

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get => $"{FirstName} {LastName}"; }

        public string Username { get; private set; }

        public string Email { get; private set; }

        public string PictureUrl { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOnUtc { get; init; }

        private readonly List<Event> _organizedEvents = new List<Event>();
        public IReadOnlyCollection<Event> OrganizedEvents => _organizedEvents;

        public void AddOrganizedEvent(Event organizedEvent) => _organizedEvents.Add(organizedEvent);
    }
}
