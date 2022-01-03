using System;
using OpenTelemetry;

namespace VF.Logging.OpenTelemetry.Configuration
{
    public class OtlpExporterDto
    {
        public Uri Endpoint { get; set; } = new("http://localhost:4317/");
        public string? Headers { get; set; }
        public int TimeoutMilliseconds { get; set; } = 15;
        public ExportProcessorType? ExportProcessorType { get; set; }
        public BatchExportProcessorOptionsDto? BatchExportProcessorOptions { get; set; }
    }
}