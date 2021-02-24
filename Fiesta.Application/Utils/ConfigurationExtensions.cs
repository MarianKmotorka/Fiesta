using Microsoft.Extensions.Configuration;

namespace Fiesta.Application.Utils
{
    public static class ConfigurationExtensions
    {
        public static bool IsTesting(this IConfiguration configuration)
        {
            return configuration.GetValue<bool>("IsTesting", defaultValue: false);
        }
    }
}
