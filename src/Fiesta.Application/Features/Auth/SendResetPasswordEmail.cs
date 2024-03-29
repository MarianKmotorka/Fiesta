﻿using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Exceptions;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Models.Emails;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fiesta.Application.Features.Auth
{
    public class SendResetPasswordEmail
    {
        public class Command : IRequest
        {
            public string Email { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IEmailService _emailService;
            private readonly IAuthService _authService;
            private readonly ILogger<Handler> _logger;

            public Handler(IEmailService emailService, IAuthService authService, ILogger<Handler> logger)
            {
                _emailService = emailService;
                _authService = authService;
                _logger = logger;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var tokenResult = await _authService.GetResetPasswordToken(request.Email, cancellationToken);
                if (tokenResult.Failed)
                    throw new BadRequestException(tokenResult.Errors);

                var sendResult = await _emailService.SendResetPasswordEmail(request.Email, new ResetPasswordEmailTemplateModel(tokenResult.Data), cancellationToken);
                if (!sendResult.Successful)
                {
                    _logger.LogError($"ResetPassword email to {request.Email} was not sent. Reason: {string.Join('\n', sendResult.ErrorMessages)}");
                    throw new BadRequestException(ErrorCodes.ServiceUnavailable);
                }

                return Unit.Value;
            }
        }
    }
}
