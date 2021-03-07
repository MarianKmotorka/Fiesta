﻿using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Validators;
using Fiesta.Application.Features.Events.CommonDtos;
using Fiesta.Domain.Entities;
using Fiesta.Domain.Entities.Events;
using FluentValidation;
using MediatR;
using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Fiesta.Application.Features.Events
{
    public class CreateEvent
    {
        public class Command : IRequest
        {
            [JsonIgnore]
            public string OrganizerId { get; set; }
            public string Name { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public AccessibilityType AccessibilityType { get; set; }
            public int Capacity { get; set; }
            public LocationDto Location { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IFiestaDbContext _fiestaDbContext;

            public Handler(IFiestaDbContext fiestaDbContext)
            {
                _fiestaDbContext = fiestaDbContext;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var fiestaUser = _fiestaDbContext.FiestaUsers.FindAsync(new[] { request.OrganizerId }, cancellationToken).Result;

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
                    request.Location.PostalCode,
                    request.Location.GoogleMapsUrl
                    );

                var organizedEvent = new Event(
                    request.Name,
                    request.StartDate.ToUniversalTime(),
                    request.EndDate.ToUniversalTime(),
                    request.AccessibilityType,
                    request.Capacity,
                    fiestaUser,
                    location
                    );

                fiestaUser.AddOrganizedEvent(organizedEvent);
                await _fiestaDbContext.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithErrorCode(ErrorCodes.Required)
                    .MinimumLength(6).WithErrorCode(ErrorCodes.MinLength).WithState(_ => new { MinLength = 6 })
                    .MaximumLength(30).WithErrorCode(ErrorCodes.MaxLength).WithState(_ => new { MaxLength = 30 });

                RuleFor(x => x.StartDate)
                    .NotEmpty().WithErrorCode(ErrorCodes.Required)
                    .GreaterThanOrEqualTo(DateTime.Now.Date).WithErrorCode(ErrorCodes.InvalidDateTime);

                RuleFor(x => x.EndDate)
                    .NotEmpty().WithErrorCode(ErrorCodes.Required)
                    .GreaterThanOrEqualTo(DateTime.Now.Date).WithErrorCode(ErrorCodes.InvalidDateTime)
                    .GreaterThanOrEqualTo(x => x.StartDate).WithErrorCode(ErrorCodes.InvalidDateTime);

                RuleFor(x => x.AccessibilityType)
                  .NotEmpty().WithErrorCode(ErrorCodes.Required)
                  .Must(Helpers.HasEnumValidValue).WithErrorCode(ErrorCodes.InvalidEnumValue);

                RuleFor(x => x.Capacity)
                  .NotEmpty().WithErrorCode(ErrorCodes.Required)
                  .GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.NegativeNumber);
            }
        }
    }
}
