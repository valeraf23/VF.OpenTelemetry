using Microsoft.Extensions.DependencyInjection;
using VF.Logging.OpenTelemetry.Configuration;
using VF.Logging.OpenTelemetry.Extensions.TracerProviderBuilderExtensions;

namespace VF.Logging.OpenTelemetry.Extensions.ServiceCollectionExtensions
{
    internal static class SamplerServiceCollectionExtensions
    {
        internal static IServiceCollection AddSampler(this IServiceCollection services)
        {
            return services.AddSingleton(p =>
            {
                var sampler = p.GetRequiredService<OpenTelemetryConfiguration>().Sampler;
                return SamplerTracerProviderBuilderExtensions.AddSampler(sampler);
            });
        }
    }
}