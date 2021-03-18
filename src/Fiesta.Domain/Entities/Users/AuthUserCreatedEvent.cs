using MediatR;

namespace Fiesta.Domain.Entities.Users
{
    public class AuthUserCreatedEvent : INotification
    {
        public AuthUserCreatedEvent(string userId, string email, string nickname)
        {
            UserId = userId;
            Email = email;
            Nickname = nickname;
        }

        public string UserId { get; }

        public string Email { get; }

        public string Nickname { get; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PictureUrl { get; set; }
    }
}
