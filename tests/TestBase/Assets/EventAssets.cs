using System;
using Fiesta.Domain.Entities;
using Fiesta.Domain.Entities.Events;
using Fiesta.Domain.Entities.Users;
using Fiesta.Infrastracture.Persistence;

namespace TestBase.Assets
{
    public static class EventAssets
    {
        public static Event SeedEvent(this FiestaDbContext db, FiestaUser organizer, Action<Event> configure = null)
        {
            var @event = new Event(
                    "New year rager",
                    new DateTime(2025, 1, 1),
                    new DateTime(2025, 1, 2),
                    AccessibilityType.Public,
                    10,
                    organizer,
                    new LocationObject(0, 0)
                );

            if (configure is not null)
                configure(@event);

            db.Events.Add(@event);
            return @event;
        }
    }
}
