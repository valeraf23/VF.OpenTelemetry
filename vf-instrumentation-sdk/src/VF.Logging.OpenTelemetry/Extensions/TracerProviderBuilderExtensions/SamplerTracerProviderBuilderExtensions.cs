using System;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;

namespace VF.Logging.OpenTelemetry.Extensions.TracerProviderBuilderExtensions
{
    internal static class SamplerTracerProviderBuilderExtensions
    {
        internal static TracerProviderBuilder RegisterSampler(this TracerProviderBuilder builder, IServiceProvider serviceProvider)
        {
            var sampler = serviceProvider.GetRequiredService<Sampler>();
            return builder.SetSampler(sampler);
        }

        internal static Sampler AddSampler(string openTelemetryConfigurations)
        {
            var key = openTelemetryConfigurations.ToUpper();
            switch (key)
            {
                case "OFF":
                    return new AlwaysOffSampler();
                case "ON":
                case "":
                    return new AlwaysOnSampler();
                default:
                    if (!double.TryParse(key.Replace(',','.'), out var ratio))
                        throw new Exception(
                            $"Invalid key:{key}. Use \"On\",\"Off\" or double value in range from 0 to 1");
                    return ratio switch
                    {
                        < 0 or > 1 => throw new Exception("Ratio have to be in range from 0 to 1"),
                        _ => new TraceIdRatioBasedSampler(ratio)
                    };
            }
        }
    }
}