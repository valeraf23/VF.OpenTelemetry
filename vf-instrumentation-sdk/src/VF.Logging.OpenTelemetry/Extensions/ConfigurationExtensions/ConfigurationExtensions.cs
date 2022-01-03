using System;
using Microsoft.Extensions.Configuration;
using VF.Logging.OpenTelemetry.Configuration;

namespace VF.Logging.OpenTelemetry.Extensions.ConfigurationExtensions
{
    internal static class ConfigurationExtensions
    {
        private const string BasePath = "VFTelemetry";

        private static string GetErrorMsg(string path) => $"Add {BasePath}:{path} section in appsettings.json";

        private static IConfigurationSection GetOpenTelemetrySection(this IConfiguration configuration)
        {
            var section = configuration.GetSection(BasePath);
            if (!section.Exists()) throw new ArgumentNullException(BasePath, GetErrorMsg(""));

            return section;
        }

        internal static OpenTelemetryConfiguration GetOpenTelemetryConfiguration(this IConfiguration configuration)
        {
            var openTelemetryConfiguration = new OpenTelemetryConfiguration();
            configuration.GetOpenTelemetrySection().Bind(openTelemetryConfiguration);
            return openTelemetryConfiguration;
        }

        internal static string[] UseInstruments(this IConfiguration configuration) =>
            configuration.GetOpenTelemetryConfiguration().Instruments;

        internal static IConfigurationSection GetTraceInstrumentationSection(this IConfiguration configuration)
        {
            const string tracePath = "TraceInstrumentation";
            return GetOpenTelemetrySection(configuration).GetSection(tracePath);
        }
    }
}