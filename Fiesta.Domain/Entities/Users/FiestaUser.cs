using Fiesta.Domain.Common;

namespace Fiesta.Domain.Entities.Users
{
    public class FiestaUser : AuditableEntity
    {
        public FiestaUser(string email)
        {
            Email = email;
        }

        public static FiestaUser CreateWithId(string id, string email)
            => new FiestaUser(email) { Id = id };

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; private set; }

        public string PictureUrl { get; set; }

        public bool IsDeleted { get; set; }
    }
}
