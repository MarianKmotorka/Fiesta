namespace Fiesta.Application.Models.Emails
{
    public class VerificationEmailTemplateModel
    {
        public VerificationEmailTemplateModel(string name, string code)
        {
            Name = name;
            Code = code;
        }

        public string Name { get; }

        public string Code { get; }
    }
}
