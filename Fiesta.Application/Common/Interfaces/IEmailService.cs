using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Models.Emails;
using FluentEmail.Core.Models;

namespace Fiesta.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task<SendResponse> SendVerificationEmail(string emailAddress, VerificationEmailTemplateModel model, CancellationToken cancellationToken);
    }
}
