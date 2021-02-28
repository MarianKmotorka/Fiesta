using System;
using System.Reflection;

namespace Fiesta.Application.Common.Queries
{
    internal static class EnumMethods
    {
        internal static MethodInfo HasFlag => typeof(Enum).GetMethod(nameof(Enum.HasFlag), new[] { typeof(Enum) });
    }
}
