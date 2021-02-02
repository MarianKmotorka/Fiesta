using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace Fiesta.Application.Auth
{
    public class Greeting
    {
        public class Command : IRequest<Response>
        {
            public string Name { get; set; }
        }

        public class Handler : IRequestHandler<Command, Response>
        {
            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new Response
                {
                    Greeting = $"Hello {request.Name}!"
                };

                return await Task.FromResult(response);
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Name).MinimumLength(2).WithErrorCode("MinLengthError").WithState(_ => new { MinLength = 2 });
            }
        }

        public class Response
        {
            public string Greeting { get; set; }
        }
    }
}
