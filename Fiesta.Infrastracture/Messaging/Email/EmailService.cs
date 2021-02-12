using Fiesta.Application.Common.Interfaces;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Fiesta.Infrastracture.Messaging.Email
{
    public class EmailService : IEmailService
    {
        private readonly string _pathToTemplate = $"./Templates/email-template.cshtml";
        private readonly IFluentEmail _fluentEmail;


        public EmailService(IFluentEmail fluentEmail)
        {
            _fluentEmail = fluentEmail;
        }

        public async Task<SendResponse> SendEmailUsingTemplate(string name, string emailAddress, string subject, CancellationToken cancellationToken)
        {
            var result = await _fluentEmail
            .To(emailAddress)
            .Subject(subject)
            .UsingTemplateFromFile(_pathToTemplate, new { Name = name })
            .SendAsync(cancellationToken);

            return result;
        }
    }
}
