using Fiesta.Application.Messaging.Email.Models;
using FluentEmail.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Fiesta.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task<SendResponse> SendVerificationEmail(string emailAddress, VerificationModel model, CancellationToken cancellationToken);
    }
}
