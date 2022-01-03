using System;
using System.Text;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.File;

namespace VF.Logging.OpenTelemetry.Logs.Serilog.Sinks.File
{
    public class LoggingConfiguration
    {
        private const string FilePath = "/logs/app.log";

        private const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss,fff} {Level:u3} trace.id={TraceId} span.id={SpanId}{NewLine}{Message:lj}{NewLine}{Exception}";

        public string Path { get; set; } = FilePath;
        public LogEventLevel RestrictedToMinimumLevel { get; set; } = LogEventLevel.Verbose;
        public string OutputTemplate { get; set; } = DefaultOutputTemplate;
        public IFormatProvider? FormatProvider { get; set; }
        public LoggingLevelSwitch? LevelSwitch { get; set; }
        public long? FileSizeLimitBytes { get; set; } = 1073741824L;
        public bool Buffered { get; set; } = false;
        public bool Shared { get; set; } = true;
        public TimeSpan? FlushToDiskInterval { get; set; }
        public RollingInterval RollingInterval { get; set; } = RollingInterval.Infinite;
        public bool RollOnFileSizeLimit { get; set; } = false;
        public int? RetainedFileCountLimit { get; set; } = 31;
        public Encoding? Encoding { get; set; }
        public FileLifecycleHooks? Hooks { get; set; }
    }
}