using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Exceptions;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Features.Auth.CommonDtos;
using Fiesta.Domain.Entities.Users;
using MediatR;

namespace Fiesta.Application.Features.Auth
{
    public class GoogleLogin
    {
        public class Command : IRequest<AuthResponse>
        {
            public string Code { get; set; }
        }

        public class Handler : IRequestHandler<Command, AuthResponse>
        {
            private readonly IAuthService _authService;
            private readonly IMediator _mediator;
            private readonly IGoogleService _googleService;

            public Handler(IAuthService authService, IMediator mediator, IGoogleService googleService)
            {
                _authService = authService;
                _mediator = mediator;
                _googleService = googleService;
            }

            public async Task<AuthResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var googleUserResult = await _googleService.GetUserInfoModelForLogin(request.Code, cancellationToken);
                if (!googleUserResult.Succeeded)
                    throw new BadRequestException(googleUserResult.Errors);

                var googleUser = googleUserResult.Data;

                var loginResult = await _authService.LoginOrRegister(googleUser, cancellationToken);
                if (loginResult.Failed)
                    throw new BadRequestException(loginResult.Errors);

                var (accessToken, refreshToken, authUserCreated, userId) = loginResult.Data;

                if (authUserCreated)
                    await _mediator.Publish(new AuthUserCreatedEvent(userId, googleUser.Email)
                    {
                        FirstName = googleUser.GivenName,
                        LastName = googleUser.FamilyName,
                        PictureUrl = googleUser.PictureUrl
                    }, cancellationToken);

                return new AuthResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };
            }
        }
    }
}
