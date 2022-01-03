using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using ILogger = Serilog.ILogger;

namespace VF.Logging.OpenTelemetry.Logs.Serilog.Sinks.File
{
    public static class Logging
    {
        private static LoggerConfiguration ReadFromConfiguration(this LoggerConfiguration logger,
            IConfiguration? configuration)
        {
            if (configuration is not null) logger.ReadFrom.Configuration(configuration);

            return logger;
        }

        public static ILogger CreateLogger() => CreateLogger(new LoggingConfiguration(), null);

        public static ILogger CreateLogger(LoggingConfiguration vfLoggingConfiguration, IConfiguration? configuration)
        {
            if (vfLoggingConfiguration is null) throw new ArgumentNullException(nameof(vfLoggingConfiguration));

            var logConfig = new LoggerConfiguration()
                .ReadFromConfiguration(configuration)
                .Enrich.With<ActivityEnricher>()
                .WriteTo.File(
                    vfLoggingConfiguration.Path,
                    vfLoggingConfiguration.RestrictedToMinimumLevel,
                    vfLoggingConfiguration.OutputTemplate,
                    vfLoggingConfiguration.FormatProvider,
                    levelSwitch: vfLoggingConfiguration.LevelSwitch,
                    fileSizeLimitBytes: vfLoggingConfiguration.FileSizeLimitBytes,
                    buffered: vfLoggingConfiguration.Buffered,
                    shared: vfLoggingConfiguration.Shared,
                    flushToDiskInterval: vfLoggingConfiguration.FlushToDiskInterval,
                    rollingInterval: vfLoggingConfiguration.RollingInterval,
                    rollOnFileSizeLimit: vfLoggingConfiguration.RollOnFileSizeLimit,
                    retainedFileCountLimit: vfLoggingConfiguration.RetainedFileCountLimit,
                    encoding: vfLoggingConfiguration.Encoding,
                    hooks: vfLoggingConfiguration.Hooks
                )
                .CreateLogger();
            return logConfig;
        }
    }
}