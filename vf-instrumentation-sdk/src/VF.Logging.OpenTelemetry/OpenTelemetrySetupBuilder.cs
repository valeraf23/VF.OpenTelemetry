using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;
using VF.Logging.OpenTelemetry.Configuration;

namespace VF.Logging.OpenTelemetry
{
    public class OpenTelemetrySetupBuilder
    {
        private readonly OpenTelemetrySetup _openTelemetrySetup;

        public OpenTelemetrySetupBuilder(IServiceCollection services)
        {
            _openTelemetrySetup = new OpenTelemetrySetup(services);
        }

        private event Action Actions = () => { };

        public OpenTelemetrySetupBuilder AddInstrumentation(Assembly assemble, params Assembly[] assemblies)
        {
            Actions += () => _openTelemetrySetup.SetInstrumentation(assemble, assemblies);
            return this;
        }

        public OpenTelemetrySetupBuilder AddTracerProvider(Func<TracerProviderBuilder, IServiceProvider, TracerProviderBuilder> tracerProviderBuilder)
        {
            Actions += () => _openTelemetrySetup.SetTracerProvider(tracerProviderBuilder);
            return this;
        }

        public OpenTelemetrySetupBuilder AddOtlpExporterOptions(Action<OtlpExporterDto> act)
        {
            Actions += () => _openTelemetrySetup.SetOtlpExporterOptions(act);
            return this;
        }

        public OpenTelemetrySetupBuilder AddService(Func<IServiceCollection, IServiceCollection> act)
        {
            Actions += () => act(_openTelemetrySetup.ServiceCollection);
            return this;
        }

        internal OpenTelemetrySetup Build()
        {
            Actions();
            return _openTelemetrySetup;
        }
    }
}