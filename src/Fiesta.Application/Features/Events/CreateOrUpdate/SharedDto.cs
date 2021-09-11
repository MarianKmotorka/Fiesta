using System;
using System.Text.Json.Serialization;
using Fiesta.Application.Features.Events.Common;
using Fiesta.Domain.Entities.Events;

namespace Fiesta.Application.Features.Events.CreateOrUpdate
{
    public class SharedDto
    {
        [JsonIgnore]
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public AccessibilityType AccessibilityType { get; set; }

        public int Capacity { get; set; }

        public LocationDto Location { get; set; }

        public string ExternalLink { get; set; }

        public string Description { get; set; }
    }
}
