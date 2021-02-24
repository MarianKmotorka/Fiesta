using System;

namespace Fiesta.Application.Common.Options
{
    public class HardDeleteUsersWorkerOptions
    {
        public TimeSpan PollingPeriod { get; set; } = TimeSpan.FromDays(1);

        public bool Enabled { get; set; }
    }
}
