namespace Fiesta.Application.Messaging.Email.Models
{
    public class VerificationModel
    {
        public VerificationModel(string name, string code)
        {
            Name = name;
            Code = code;
        }
        public string Name { get; }
        public string Code { get; }
    }
}
