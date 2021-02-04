using Fiesta.Application.Auth.CommonDtos;
using MediatR;

namespace Fiesta.Application.Auth.GoogleLogin
{
    public class GoogleLoginCommand : IRequest<AuthResponse>
    {
        public string Code { get; set; }
    }
}
