using MediatR;

namespace Fiesta.Domain.Entities.Users
{
    public class AuthUserCreatedEvent : INotification
    {
        public AuthUserCreatedEvent(string userId, string email, string username)
        {
            UserId = userId;
            Email = email;
            Username = username;
        }

        public string UserId { get; }

        public string Email { get; }

        public string Username { get; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PictureUrl { get; set; }
    }
}
