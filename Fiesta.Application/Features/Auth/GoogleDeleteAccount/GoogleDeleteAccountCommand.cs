using MediatR;

namespace Fiesta.Application.Features.Auth.GoogleDeleteAccount
{
    public class GoogleDeleteAccountCommand : IRequest<Unit>
    {
        public string Code { get; set; }
    }
}