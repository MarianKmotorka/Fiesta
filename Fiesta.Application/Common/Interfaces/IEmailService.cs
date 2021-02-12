using FluentEmail.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Fiesta.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task<SendResponse> SendEmailUsingTemplate(string name, string emailAddress, string subject, CancellationToken cancellationToken);
    }
}
