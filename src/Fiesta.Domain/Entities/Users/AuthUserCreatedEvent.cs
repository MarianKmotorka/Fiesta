using MediatR;

namespace Fiesta.Domain.Entities.Users
{
    public class AuthUserCreatedEvent : INotification
    {
        public AuthUserCreatedEvent(string userId, string email)
        {
            UserId = userId;
            Email = email;
        }

        public string UserId { get; }

        public string Email { get; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PictureUrl { get; set; }
    }
}
