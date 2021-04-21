namespace Fiesta.Application.Features.Common
{
    public class UserDto
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string PictureUrl { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
