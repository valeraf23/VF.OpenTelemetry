using OpenTelemetry.Trace;

namespace VF.Logging.OpenTelemetry.TraceInstrumentation
{
    public interface IInstrumentation
    {
        TracerProviderBuilder Add(TracerProviderBuilder builder);
    }
}