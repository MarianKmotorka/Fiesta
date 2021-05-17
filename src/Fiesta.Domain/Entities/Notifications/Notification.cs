using System;
using Fiesta.Domain.Common;
using Fiesta.Domain.Entities.Users;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Fiesta.Domain.Entities.Notifications
{
    public class Notification : Entity<long>
    {
        private Notification()
        {
        }

        public Notification(FiestaUser user, INotificationModel model) : this(user.Id, model)
        {
            User = user;
        }

        public Notification(string userId, INotificationModel model)
        {
            Type = model.Type;
            UserId = userId;
            CreatedAt = DateTime.UtcNow;

            Model = JsonConvert.SerializeObject(
                model,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }
                );
        }

        public NotificationType Type { get; private init; }

        public bool Seen { get; private set; }

        public DateTime CreatedAt { get; private init; }

        public string Model { get; private init; }

        public FiestaUser User { get; private set; }

        public string UserId { get; private set; }

        public T GetModel<T>() where T : new()
        {
            return JsonConvert.DeserializeObject<T>(Model);
        }

        public void SetSeen()
        {
            Seen = true;
        }
    }
}
