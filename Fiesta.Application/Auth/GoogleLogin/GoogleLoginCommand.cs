using MediatR;

namespace Fiesta.Application.Auth.GoogleLogin
{
    public class GoogleLoginCommand : IRequest<GoogleLoginResponse>
    {
        public string Code { get; set; }
    }
}
