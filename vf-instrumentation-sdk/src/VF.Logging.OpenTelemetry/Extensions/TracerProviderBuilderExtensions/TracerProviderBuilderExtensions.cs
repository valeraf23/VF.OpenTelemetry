using OpenTelemetry.Trace;

namespace VF.Logging.OpenTelemetry.Extensions.TracerProviderBuilderExtensions
{
    public static class VfTracerProviderBuilderExtensions
    {
        private const string Key = "VFDiagnosticListener";

        public static TracerProviderBuilder AddSource(this TracerProviderBuilder builder) => builder.AddSource(Key);

        public static Tracer GetTracer(this TracerProvider builder) => builder.GetTracer(Key);
    }
}