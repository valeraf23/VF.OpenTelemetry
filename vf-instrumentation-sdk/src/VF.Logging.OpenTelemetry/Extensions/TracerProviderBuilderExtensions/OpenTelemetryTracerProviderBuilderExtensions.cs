using System;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using VF.Logging.OpenTelemetry.Configuration;

namespace VF.Logging.OpenTelemetry.Extensions.TracerProviderBuilderExtensions
{
    internal static class OpenTelemetryTracerProviderBuilderExtensions
    {
        internal static TracerProviderBuilder AddTracing(this TracerProviderBuilder builder, IServiceProvider serviceProvider, string applicationName)
        {

            var dto = serviceProvider.GetRequiredService<OtlpExporterDto>();

            return VfTracerProviderBuilderExtensions.AddSource(builder
                    .AddTraceInstrumentation(serviceProvider)
                    .RegisterSampler(serviceProvider)
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(applicationName)))
                .AddOtlpExporter(options => options.MapFrom(dto));
        }

    }
}