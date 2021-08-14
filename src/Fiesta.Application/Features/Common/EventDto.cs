using System;
using Fiesta.Domain.Entities.Events;

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
    }
}
