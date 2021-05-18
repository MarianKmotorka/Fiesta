using System;
using Fiesta.Application.Common.Interfaces;

namespace Fiesta.Infrastracture
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
