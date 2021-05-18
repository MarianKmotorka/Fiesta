using System;

namespace Fiesta.Application.Common.Options
{
    public class NotificationDeletionWorkerOptions
    {
        public TimeSpan PollingPeriod { get; set; } = TimeSpan.FromDays(1);

        public bool Enabled { get; set; }

        public TimeSpan DeleteAfter { get; set; } = TimeSpan.FromDays(30);

        public int BatchSize { get; set; } = 1000;
    }
}
