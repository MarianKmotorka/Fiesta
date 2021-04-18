using System;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Validators;
using Fiesta.Application.Utils;
using Fiesta.Domain.Entities;
using Fiesta.Domain.Entities.Events;
using FluentValidation;
using MediatR;

namespace Fiesta.Application.Features.Events.CreateOrUpdate
{
    public class CreateOrUpdateEvent
    {
        public class Command : SharedDto, IRequest<Response>
        {
        }

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                var location = new LocationObject(
                    request.Location.Latitude,
                    request.Location.Longitude,
                    request.Location.Street,
                    request.Location.StreetNumber,
                    request.Location.Premise,
                    request.Location.City,
                    request.Location.State,
                    request.Location.AdministrativeAreaLevel1,
                    request.Location.AdministrativeAreaLevel2,
                    request.Location.PostalCode
                    );

                Event @event;
                if (request.Id == default)
                {
                    var organizer = await _db.FiestaUsers.FindAsync(new[] { request.OrganizerId }, cancellationToken);
                    @event = _db.Events.Add(new Event(
                        request.Name,
                        request.StartDate.ToUniversalTime(),
                        request.EndDate.ToUniversalTime(),
                        request.AccessibilityType,
                        request.Capacity,
                        organizer,
                        location
                    )).Entity;
                }
                else
                    @event = await _db.Events.FindOrNotFoundAsync(cancellationToken, request.Id);

                @event.SetDescription(request.Description);
                @event.Location = location;
                @event.StartDate = request.StartDate;
                @event.EndDate = request.EndDate;
                @event.Capacity = request.Capacity;
                @event.AccessibilityType = request.AccessibilityType;
                await _db.SaveChangesAsync(cancellationToken);

                return new Response
                {
                    Id = @event.Id
                };
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithErrorCode(ErrorCodes.Required)
                    .MaximumLength(30).WithErrorCode(ErrorCodes.MaxLength).WithState(_ => new { MaxLength = 30 });

                RuleFor(x => x.StartDate)
                    .NotEmpty().WithErrorCode(ErrorCodes.Required)
                    .GreaterThanOrEqualTo(DateTime.Now.Date).WithErrorCode(ErrorCodes.InvalidDateTime);

                RuleFor(x => x.EndDate)
                    .NotEmpty().WithErrorCode(ErrorCodes.Required)
                    .GreaterThanOrEqualTo(DateTime.Now.Date).WithErrorCode(ErrorCodes.InvalidDateTime)
                    .GreaterThanOrEqualTo(x => x.StartDate).WithErrorCode(ErrorCodes.InvalidDateTime);

                RuleFor(x => x.AccessibilityType)
                  .NotNull().WithErrorCode(ErrorCodes.Required)
                  .HasEnumValidValue();

                RuleFor(x => x.Capacity)
                  .NotEmpty().WithErrorCode(ErrorCodes.Required)
                  .GreaterThanOrEqualTo(2).WithErrorCode(ErrorCodes.Min).WithState(_ => new { Min = 2 });

                RuleFor(x => x.Location)
                    .Must(x => LocationObject.ValidateLatitudeAndLongitude(x.Latitude, x.Longitude)).WithErrorCode(ErrorCodes.InvalidLatitudeOrLongitude);

                RuleFor(x => x.Description)
                    .MaximumLength(2000).WithErrorCode(ErrorCodes.MaxLength).WithState(_ => new { MaxLength = 2000 });
            }
        }

        public class Response
        {
            public string Id { get; set; }
        }
    }
}
