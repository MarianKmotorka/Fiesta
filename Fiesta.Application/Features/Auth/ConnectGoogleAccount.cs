using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Exceptions;
using Fiesta.Application.Common.Interfaces;
using MediatR;

namespace Fiesta.Application.Features.Auth
{
    public class ConnectGoogleAccount
    {
        public class Command : IRequest
        {
            [JsonIgnore]
            public string UserId { get; set; }

            public string Code { get; set; }
        }

        public class Handler : IRequestHandler<Command>
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
                var googleResult = await _googleService.GetUserInfoModel(request.Code, cancellationToken);
                if (googleResult.Failed)
                    throw new BadRequestException(googleResult.Errors);

                var result = await _authService.AddGoogleAccount(request.UserId, googleResult.Data, cancellationToken);
                if (result.Failed)
                    throw new BadRequestException(result.Errors);

                return Unit.Value;
            }
        }
    }
}
