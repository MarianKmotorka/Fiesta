using System.Reflection;

namespace Fiesta.Application.Common.Queries
{
    internal static class StringMethods
    {
        internal static MethodInfo StartsWith { get; } = typeof(string).GetMethod(nameof(string.StartsWith), new[] { typeof(string) });
        internal static MethodInfo Contains { get; } = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });
        internal static MethodInfo EndsWith { get; } = typeof(string).GetMethod(nameof(string.EndsWith), new[] { typeof(string) });
        internal new static MethodInfo Equals { get; } = typeof(string).GetMethod(nameof(string.Equals), new[] { typeof(string) });
    }
}
