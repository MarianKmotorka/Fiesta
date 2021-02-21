using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Exceptions;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Features.Auth.CommonDtos;
using Fiesta.Domain.Entities.Users;
using MediatR;

namespace Fiesta.Application.Features.Auth.GoogleLogin
{
    public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, AuthResponse>
    {
        private readonly IAuthService _authService;
        private readonly IMediator _mediator;
        private readonly IGoogleService _googleService;

        public GoogleLoginCommandHandler(IAuthService authService, IMediator mediator, IGoogleService googleService)
        {
            _authService = authService;
            _mediator = mediator;
            _googleService = googleService;
        }

        public async Task<AuthResponse> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        {
            var googleUserResult = await _googleService.GetUserInfoModel(request.Code, cancellationToken);

            if (!googleUserResult.Succeeded)
                throw new BadRequestException(googleUserResult.Errors);

            var googleUser = googleUserResult.Data;
            var (accessToken, refreshToken, authUserCreated, userId) = await _authService.LoginOrRegister(googleUser, cancellationToken);

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
