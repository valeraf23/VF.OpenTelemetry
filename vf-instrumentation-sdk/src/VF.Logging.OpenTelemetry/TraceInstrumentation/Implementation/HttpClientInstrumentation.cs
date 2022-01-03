using OpenTelemetry.Trace;

namespace VF.Logging.OpenTelemetry.TraceInstrumentation.Implementation
{
    public class HttpClientInstrumentation : IInstrumentation
    {
        public TracerProviderBuilder Add(TracerProviderBuilder builder) => builder
            .AddHttpClientInstrumentation();
    }
}