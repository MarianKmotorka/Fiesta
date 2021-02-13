using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Messaging.Email.Constants;
using Fiesta.Application.Messaging.Email.Models;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;

namespace Fiesta.Infrastracture.Messaging.Email
{
    public class EmailService : IEmailService
    {
        private readonly string _pathToTemplates = $"Fiesta.Infrastracture.Messaging.Email.Templates.";
        private readonly IFluentEmail _fluentEmail;


        public EmailService(IFluentEmail fluentEmail)
        {
            _fluentEmail = fluentEmail;
        }

        public async Task<SendResponse> SendVerificationEmail(string emailAddress, VerificationModel model, CancellationToken cancellationToken)
        {
            var result = await BuildEmailUsingTemplate(emailAddress, EmailSubjects.VerificationEmail, TemplateNames.VerificationEmail, model).SendAsync(cancellationToken);

            return result;
        }

        private IFluentEmail BuildEmailUsingTemplate(string emailAddress, string subject, string template, object model)
        {
            var email = _fluentEmail
            .To(emailAddress)
            .Subject(subject)
            .UsingTemplateFromEmbedded(_pathToTemplates + template, JObject.FromObject(model), GetType().Assembly);

            return email;
        }
    }
}
