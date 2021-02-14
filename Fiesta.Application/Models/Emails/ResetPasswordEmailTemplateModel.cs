namespace Fiesta.Application.Models.Emails
{
    public class ResetPasswordEmailTemplateModel
    {
        public ResetPasswordEmailTemplateModel(string token)
        {
            Token = token;
        }

        public string Token { get; }
    }
}
