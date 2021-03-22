using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Fiesta.Application.Features.Selectors
{
    public class EventsAndUsersSelector
    {
        public class Query : IRequest<List<ResponseDto>>
        {
            public string Search { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<ResponseDto>>
        {
            private readonly IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<List<ResponseDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                // TODO: only show public events or that user is part of

                var usersQuery = _db.FiestaUsers.AsNoTracking()
                    .Where(x => !x.IsDeleted)
                    .Select(x => new ResponseDto
                    {
                        Id = x.Id,
                        DisplayName = x.Username,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        PictureUrl = x.PictureUrl,
                        Type = ItemType.User,
                        StartDate = null,
                        Description = null,
                        City = null,
                        State = null
                    });

                var eventsQuery = _db.Events.AsNoTracking().Select(x => new ResponseDto
                {
                    Id = x.Id,
                    DisplayName = x.Name,
                    Description = "Some hard coded description, bla bla bla, ble ble ell, mengeleho deto sa bude smazit",
                    StartDate = x.StartDate,
                    Type = ItemType.Event,
                    City = x.Location.City,
                    State = x.Location.State,
                    FirstName = null,
                    LastName = null,
                    PictureUrl = null,
                });

                var query = usersQuery.Union(eventsQuery);

                if (!string.IsNullOrEmpty(request.Search))
                    query = query.Where(x => x.DisplayName.Contains(request.Search) || (x.FirstName + " " + x.LastName).Contains(request.Search));

                return await query.OrderBy(x => x.DisplayName).Take(25).ToListAsync(cancellationToken);
            }
        }

        public class ResponseDto
        {
            public string Id { get; set; }

            public string DisplayName { get; set; }

            public string FullName { get => FirstName is null ? null : $"{FirstName} {LastName}"; }

            public string Description { get; set; }

            public DateTime? StartDate { get; set; }

            public string PictureUrl { get; set; }

            public string Location { get => City is null ? null : $"{City}, {State}"; }

            public ItemType Type { get; set; }

            [JsonIgnore]
            public string FirstName { get; set; }

            [JsonIgnore]
            public string LastName { get; set; }

            [JsonIgnore]
            public string City { get; set; }

            [JsonIgnore]
            public string State { get; set; }
        }

        public enum ItemType
        {
            Event = 0,
            User = 1
        }
    }
}
