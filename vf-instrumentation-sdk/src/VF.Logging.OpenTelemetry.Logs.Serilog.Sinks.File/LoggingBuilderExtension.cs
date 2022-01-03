using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System;

namespace VF.Logging.OpenTelemetry.Logs.Serilog.Sinks.File
{
    public static class LoggingBuilderExtension
    {
        public static ILoggingBuilder AddVfLog(this ILoggingBuilder builder, Action<LoggingConfiguration> func, bool dispose = false)
        {
            var vfLoggingConfiguration = new LoggingConfiguration();
            func(vfLoggingConfiguration);
            return AddVfLog(builder, vfLoggingConfiguration, dispose);
        }

        public static ILoggingBuilder AddVfLog(this ILoggingBuilder builder, LoggingConfiguration configuration, bool dispose = false)
            => AddVfLog(builder, configuration, null, dispose);

        public static ILoggingBuilder AddVfLog(this ILoggingBuilder builder, LoggingConfiguration vfLoggingConfiguration, IConfiguration? configuration, bool dispose = false)
        {
            Log.Logger = Logging.CreateLogger(vfLoggingConfiguration, configuration);
            builder.AddSerilog(Log.Logger, dispose);
            return builder;
        }

        public static ILoggingBuilder AddVfLog(this ILoggingBuilder builder, IConfiguration configuration, bool dispose = false)
        {
            var vfLoggingConfiguration = configuration.GetSection("VFLogging").Get<LoggingConfiguration>();
            vfLoggingConfiguration ??= new LoggingConfiguration();

            return AddVfLog(builder, vfLoggingConfiguration, configuration, dispose);
        }

        public static ILoggingBuilder AddVfLog(this ILoggingBuilder builder, bool dispose = false)
            => AddVfLog(builder, new LoggingConfiguration(), null, dispose);
    }
}