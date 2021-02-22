using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Exceptions;
using Fiesta.Application.Common.Interfaces;
using MediatR;

namespace Fiesta.Application.Features.Auth
{
    public class DeleteAccountWithGoogle
    {
        public class Command : IRequest<Unit>
        {
            [JsonIgnore]
            public string UserId { get; set; }
            public string Code { get; set; }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly IAuthService _authService;
            private readonly IGoogleService _googleService;

            public Handler(IAuthService authService, IGoogleService googleService)
            {
                _authService = authService;
                _googleService = googleService;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var googleUserResult = await _googleService.GetUserInfoModel(request.Code, cancellationToken);

                if (!googleUserResult.Succeeded)
                    throw new BadRequestException(googleUserResult.Errors);

                var googleUser = googleUserResult.Data;

                await _authService.DeleteAccountWithGoogle(request.UserId, googleUser, cancellationToken);

                return Unit.Value;
            }
        }
    }
}
