using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;
using VF.Logging.OpenTelemetry.Configuration;
using VF.Logging.OpenTelemetry.Extensions.ConfigurationExtensions;
using VF.Logging.OpenTelemetry.Extensions.ServiceCollectionExtensions;
using VF.Logging.OpenTelemetry.Extensions.TracerProviderBuilderExtensions;

namespace VF.Logging.OpenTelemetry
{
    public static class OpenTelemetryServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenTelemetry(this IServiceCollection services,
            IConfiguration configuration, string applicationName)
        {
            return AddOpenTelemetry(services, configuration, applicationName, b => b);
        }

        public static IServiceCollection AddOpenTelemetry(this IServiceCollection services,
            IConfiguration configuration, string applicationName,
            Func<OpenTelemetrySetupBuilder, OpenTelemetrySetupBuilder> setupBuilder)
        {
            var openTelemetrySetup = setupBuilder(new OpenTelemetrySetupBuilder(services)).Build();

            var otlpExporterDto =
                configuration.GetSection("VFTelemetry:OtlpExporterOptions").Get<OtlpExporterDto>() ??
                new OtlpExporterDto();

            return openTelemetrySetup
                .AddExporterOption(otlpExporterDto)
                .AddInstrumentation(configuration.UseInstruments())
                .ServiceCollection
                .AddConfiguration(configuration)
                .AddSampler()
                .AddOpenTelemetryTracing(builder =>
                {
                    builder.Configure(
                        (serviceProvider, b) =>
                        {
                            openTelemetrySetup
                                .AddTracerProvider(b, serviceProvider)
                                .AddTracing(serviceProvider, applicationName);
                        });
                })
                .AddTracer();
        }
    }
}