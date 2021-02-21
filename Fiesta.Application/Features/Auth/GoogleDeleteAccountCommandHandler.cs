using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Exceptions;
using Fiesta.Application.Common.Interfaces;
using MediatR;

namespace Fiesta.Application.Features.Auth
{
    public class GoogleDeleteAccountCommandHandler : IRequestHandler<GoogleDeleteAccountCommand, Unit>
    {
        private readonly IAuthService _authService;
        private readonly IGoogleService _googleService;

        public GoogleDeleteAccountCommandHandler(IAuthService authService, IGoogleService googleService)
        {
            _authService = authService;
            _googleService = googleService;
        }

        public async Task<Unit> Handle(GoogleDeleteAccountCommand request, CancellationToken cancellationToken)
        {
            var googleUserResult = await _googleService.GetUserInfoModel(request.Code, cancellationToken);

            if (!googleUserResult.Succeeded)
                throw new BadRequestException(googleUserResult.Errors);

            var googleUser = googleUserResult.Data;

            await _authService.DeleteAccountWithGoogle(googleUser.Email, cancellationToken);

            return Unit.Value;
        }
    }

    public class GoogleDeleteAccountCommand : IRequest<Unit>
    {
        public string Code { get; set; }
    }
}
