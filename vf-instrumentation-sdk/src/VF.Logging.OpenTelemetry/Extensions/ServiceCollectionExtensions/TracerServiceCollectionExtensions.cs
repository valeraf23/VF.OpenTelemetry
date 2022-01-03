using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;
using VF.Logging.OpenTelemetry.Extensions.TracerProviderBuilderExtensions;
using VF.Logging.OpenTelemetry.VfTracer;


namespace VF.Logging.OpenTelemetry.Extensions.ServiceCollectionExtensions
{
    internal static class TracerServiceCollectionExtensions
    {
        internal static IServiceCollection AddTracer(this IServiceCollection services)
        {
            return services.AddTransient((System.Func<System.IServiceProvider, ITracer>)(x =>
                new VfTracer.Tracer(x.GetRequiredService<TracerProvider>().GetTracer())));
        }
    }
}