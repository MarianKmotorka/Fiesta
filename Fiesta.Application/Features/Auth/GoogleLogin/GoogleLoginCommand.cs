using Fiesta.Application.Features.Auth.CommonDtos;
using MediatR;

namespace Fiesta.Application.Features.Auth.GoogleLogin
{
    public class GoogleLoginCommand : IRequest<AuthResponse>
    {
        public string Code { get; set; }
    }
}
