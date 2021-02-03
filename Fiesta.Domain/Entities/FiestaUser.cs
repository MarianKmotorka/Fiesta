using Fiesta.Domain.Common;

namespace Fiesta.Domain.Entities
{
    public class FiestaUser : AuditableEntity
    {
        public FiestaUser(string email)
        {
            Email = email;
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; private set; }

        public string PictureUrl { get; set; }
    }
}
