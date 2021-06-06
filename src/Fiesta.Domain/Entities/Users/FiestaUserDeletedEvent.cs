using System;
using MediatR;

namespace Fiesta.Domain.Entities.Users
{
    public class FiestaUserDeletedEvent : INotification
    {
        public FiestaUserDeletedEvent(FiestaUser deletedUser)
        {
            User = deletedUser ?? throw new ArgumentNullException(nameof(deletedUser));
        }

        public FiestaUser User { get; }
    }
}
