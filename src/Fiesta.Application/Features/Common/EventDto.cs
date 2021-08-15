using System;
using Fiesta.Domain.Entities.Events;
using Newtonsoft.Json;

namespace Fiesta.Application.Features.Common
{
    public class EventDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string BannerUrl { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public AccessibilityType AccessibilityType { get; set; }

        public string Location { get => City is null ? null : $"{City}, {State}"; }

        [JsonIgnore]
        public string City { get; set; }

        [JsonIgnore]
        public string State { get; set; }
    }
}
