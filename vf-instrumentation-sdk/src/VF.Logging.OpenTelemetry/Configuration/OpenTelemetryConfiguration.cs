using VF.Logging.OpenTelemetry.TraceInstrumentation.Implementation;

namespace VF.Logging.OpenTelemetry.Configuration
{
    public class OpenTelemetryConfiguration
    {
        public OtlpExporterDto OtlpExporterOptions { get; set; } = new();
        public string Sampler { get; set; } = "on";
        public string[] Instruments { get; set; } = { nameof(AspNetCoreInstrumentation) };
    }
}