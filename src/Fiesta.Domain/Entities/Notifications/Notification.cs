﻿using System;
using Fiesta.Domain.Common;
using Fiesta.Domain.Entities.Users;
using Newtonsoft.Json;

namespace Fiesta.Domain.Entities.Notifications
{
    public class Notification : Entity<long>
    {
        private Notification()
        {
        }

        public Notification(NotificationType type, FiestaUser user) : this(type, user, null)
        {
        }

        public Notification(NotificationType type, FiestaUser user, object model)
        {
            Type = type;
            User = user;
            UserId = user.Id;
            Model = JsonConvert.SerializeObject(model);
            CreatedAt = DateTime.UtcNow;
        }

        public NotificationType Type { get; private init; }

        public bool Seen { get; private set; }

        public DateTime CreatedAt { get; private init; }

        public string Model { get; private init; }

        public FiestaUser User { get; private set; }

        public string UserId { get; private set; }

        public T GetModel<T>() where T : class
        {
            return JsonConvert.DeserializeObject<T>(Model);
        }

        public void SetSeen()
        {
            Seen = true;
        }
    }
}
