using System;
using System.Threading.Tasks;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using VF.Logging.OpenTelemetry.Extensions.TracerProviderBuilderExtensions;

namespace VF.Logging.OpenTelemetry
{
    public static class Sdk
    {
        public static void AddSpan(Action act)
        {
            var tracer = new VfTracer.Tracer(TracerProvider.Default.GetTracer());
            tracer.AddSpan(act);
        }

        public static async Task AddSpan(Func<Task> act)
        {
            var tracer = new VfTracer.Tracer(TracerProvider.Default.GetTracer());
            await tracer.AddSpan(act);
        }

        public static void AddSpan(string spanName, Action act)
        {
            var tracer = new VfTracer.Tracer(TracerProvider.Default.GetTracer());
            tracer.AddSpan(spanName, act);
        }

        public static async Task AddSpan(string spanName, Func<Task> act)
        {
            var tracer = new VfTracer.Tracer(TracerProvider.Default.GetTracer());
            await tracer.AddSpan(spanName, act);
        }

        public static TelemetrySpan StartActiveSpan(string name, SpanKind kind = SpanKind.Internal)
        {
            var tracer = new VfTracer.Tracer(TracerProvider.Default.GetTracer());
            return tracer.GetTracer().StartActiveSpan(name, kind);
        }

        public static TracerProviderBuilder CreateTracerProviderBuilder(string serviceName, string otlpExporterEndpoint)
        {
            return CreateTracerProviderBuilder(serviceName, new Uri(otlpExporterEndpoint));
        }

        public static TracerProviderBuilder CreateTracerProviderBuilder(string serviceName, Uri otlpExporterEndpoint)
        {
            return CreateTracerProviderBuilder(ResourceBuilder.CreateDefault().AddService(serviceName),
                otlpExporterEndpoint);
        }

        public static TracerProviderBuilder CreateTracerProviderBuilder(ResourceBuilder resourceBuilder, Uri otlpExporterEndpoint)
        {
            return VfTracerProviderBuilderExtensions.AddSource(global::OpenTelemetry.Sdk.CreateTracerProviderBuilder())
                .SetResourceBuilder(resourceBuilder)
                .AddOtlpExporter(x => x.Endpoint = otlpExporterEndpoint);
        }
    }
}