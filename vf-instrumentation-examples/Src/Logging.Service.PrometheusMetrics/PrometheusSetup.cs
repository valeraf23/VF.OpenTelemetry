using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Exporter.Prometheus;
using OpenTelemetry.Metrics;
using OpenTelemetry.Metrics.Export;

namespace PrometheusMetrics
{
    public static class PrometheusSetup
    {
        public static IServiceCollection AddPrometheusMetrics(this IServiceCollection services,
            string prometheusExporterOptions = null)
        {
            //services.AddHostedService(b =>
            //    new PrometheusMetricsHostedService(
            //        b.GetRequiredService<ILogger<PrometheusMetricsHostedService>>(),
            //        TimeSpan.FromSeconds(1)));

            var promOptions = new PrometheusExporterOptions { Url = prometheusExporterOptions };
            var promExporter = new PrometheusExporter(promOptions);

            services.AddSingleton(promExporter);

            MeterProvider.SetDefault(Sdk.CreateMeterProviderBuilder()
                .SetProcessor(new UngroupedBatcher())
                .SetExporter(promExporter)
                .SetPushInterval(TimeSpan.FromSeconds(1))
                .Build());

            return services;
        }

        public static IApplicationBuilder UsePrometheusMetrics(this IApplicationBuilder app)
        {
            const string defaultPath = "/metrics";
            var options =
                app.ApplicationServices.GetService(typeof(PrometheusExporterOptions)) as PrometheusExporterOptions;
            var path = new PathString(options?.Url ?? defaultPath);
            return app.Map(
                path,
                (a) => a.UseMiddleware<PrometheusExporterMiddleware>());
        }
    }
}