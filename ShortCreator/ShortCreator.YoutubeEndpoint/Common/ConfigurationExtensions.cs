
using System.Collections.Specialized;

namespace ShortCreator.YoutubeEndpoint.Common
{
    public static class ConfigurationExtensions
    {
        // Retrieves a required setting, throws an exception if missing
        public static string Required(this IConfiguration configuration, string key)
        {
            var value = configuration[key];
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException($"Missing required configuration key: {key}");
            }
            return value;
        }

        // Retrieves a setting or returns the provided default value
        public static string ValueOrDefault(this IConfiguration configuration, string key, string defaultValue)
        {
            return configuration[key] ?? defaultValue;
        }
    }

}
