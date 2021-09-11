using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Validators;
using Fiesta.Application.Features.Events.Common;
using Fiesta.Application.Utils;
using Fiesta.Domain.Entities;
using Fiesta.Domain.Entities.Events;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Events.CreateOrUpdate
{
    public class CreateOrUpdateEvent
    {
        public class Command : SharedDto, IRequest<Response>
        {
            [JsonIgnore]
            public string OrganizerId { get; set; }
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
                var location = GetLocationOrDefault(request);

                Event @event;
                if (request.Id == default)
                {
                    var organizer = await _db.FiestaUsers.FindAsync(new[] { request.OrganizerId }, cancellationToken);
                    @event = location is null
                        ? new Event(
                            request.Name,
                            request.StartDate.ToUniversalTime(),
                            request.EndDate.ToUniversalTime(),
                            request.AccessibilityType,
                            request.Capacity,
                            organizer,
                            request.ExternalLink
                        )
                        : new Event(
                            request.Name,
                            request.StartDate.ToUniversalTime(),
                            request.EndDate.ToUniversalTime(),
                            request.AccessibilityType,
                            request.Capacity,
                            organizer,
                            location
                        );
                    _db.Events.Add(@event);
                }
                else
                    @event = await _db.Events.FindOrNotFoundAsync(cancellationToken, request.Id);

                @event.Name = request.Name;
                @event.StartDate = request.StartDate;
                @event.EndDate = request.EndDate;
                @event.Capacity = request.Capacity;
                @event.AccessibilityType = request.AccessibilityType;
                @event.SetDescription(request.Description);

                if (location is not null)
                    @event.SetLocation(location);
                else
                    @event.SetExternalLink(request.ExternalLink);

                await _db.SaveChangesAsync(cancellationToken);

                return new Response
                {
                    Id = @event.Id
                };
            }

            private LocationObject GetLocationOrDefault(Command request)
            {
                if (request.Location is null)
                    return null;

                return new LocationObject(
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
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly IFiestaDbContext _db;

            public Validator(IFiestaDbContext db)
            {
                _db = db;

                RuleFor(x => x.Name)
                    .NotEmpty().WithErrorCode(ErrorCodes.Required)
                    .MaximumLength(30).WithErrorCode(ErrorCodes.MaxLength).WithState(_ => new { MaxLength = 30 });

                RuleFor(x => x.StartDate)
                    .NotEmpty().WithErrorCode(ErrorCodes.Required)
                    .GreaterThanOrEqualTo(DateTime.Now.Date).WithErrorCode(ErrorCodes.MustBeInTheFuture);

                RuleFor(x => x.EndDate)
                    .NotEmpty().WithErrorCode(ErrorCodes.Required)
                    .GreaterThanOrEqualTo(DateTime.Now.Date).WithErrorCode(ErrorCodes.MustBeInTheFuture)
                    .GreaterThanOrEqualTo(x => x.StartDate).WithErrorCode(ErrorCodes.MustBeAfterStartDate);

                RuleFor(x => x.AccessibilityType)
                  .NotNull().WithErrorCode(ErrorCodes.Required)
                  .HasEnumValidValue();

                RuleFor(x => x.Capacity)
                  .Cascade(CascadeMode.Stop)
                  .NotEmpty().WithErrorCode(ErrorCodes.Required)
                  .GreaterThanOrEqualTo(2).WithErrorCode(ErrorCodes.Min).WithState(_ => new { Min = 2 })
                  .MustAsync(GreaterThanNumberOfAttendees).WithErrorCode(ErrorCodes.CannotBeLessThanCurrentAttendeesCount);

                RuleFor(x => x.Location)
                    .Must(x => LocationObject.ValidateLatitudeAndLongitude(x.Latitude, x.Longitude)).When(x => x.Location is not null).WithErrorCode(ErrorCodes.InvalidLatitudeOrLongitude)
                    .Must(EitherExternalLinkOrLocationIsSet).WithErrorCode(ErrorCodes.EitherExternalLinkOrLocationMustBeSet);

                RuleFor(x => x.ExternalLink)
                    .Matches(@"[(http(s)?):\/\/(www\.)?a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)").WithErrorCode(ErrorCodes.Invalid);

                RuleFor(x => x.Description)
                    .MaximumLength(2000).WithErrorCode(ErrorCodes.MaxLength).WithState(_ => new { MaxLength = 2000 });
            }

            private bool EitherExternalLinkOrLocationIsSet(Command command, LocationDto _)
            {
                if (command.Location is null && string.IsNullOrEmpty(command.ExternalLink))
                    return false;

                if (command.Location is not null && !string.IsNullOrEmpty(command.ExternalLink))
                    return false;

                return true;
            }

            private async Task<bool> GreaterThanNumberOfAttendees(Command command, int capacity, CancellationToken cancellationToken)
            {
                if (command.Id == default)
                    return true;

                var attendeesCount = await _db.Events.Where(x => x.Id == command.Id).SelectMany(x => x.Attendees).CountAsync(cancellationToken);
                return command.Capacity >= attendeesCount;
            }
        }

        public class Response
        {
            public string Id { get; set; }
        }

        public class AuthorizationCheck : IAuthorizationCheck<Command>
        {
            public async Task<bool> IsAuthorized(Command request, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken)
            {
                if (request.Id == default)
                    return true;

                var resource = await db.Events.FindOrNotFoundAsync(cancellationToken, request.Id);
                return currentUserService.IsResourceOwnerOrAdmin(resource.OrganizerId);
            }
        }
    }
}
