using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;
using VF.Logging.OpenTelemetry.Configuration;
using VF.Logging.OpenTelemetry.Extensions.ServiceCollectionExtensions;

namespace VF.Logging.OpenTelemetry
{
    public class OpenTelemetrySetup
    {
        public OpenTelemetrySetup(IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
            AddInstrumentation = keys =>
            {
                ServiceCollection.AddInstrumentation(keys);
                return this;
            };

            AddExporterOption = options =>
            {
                ServiceCollection.AddSingleton(options);
                return this;
            };
        }

        internal IServiceCollection ServiceCollection { get; }
        internal Func<IEnumerable<string>, OpenTelemetrySetup> AddInstrumentation { get; private set; }

        internal Func<TracerProviderBuilder, IServiceProvider, TracerProviderBuilder> AddTracerProvider { get; private set; } = (builder, _) => builder;

        internal Func<OtlpExporterDto, OpenTelemetrySetup> AddExporterOption { get; private set; }

        internal void SetInstrumentation(Assembly? assemble, params Assembly[] assemblies)
        {
            if (assemble is null)
            {
                throw new ArgumentNullException(nameof(assemble));
            }

            AddInstrumentation = keys =>
            {
                ServiceCollection.AddInstrumentation(keys, assemble, assemblies);
                return this;
            };
        }

        internal void SetTracerProvider(Func<TracerProviderBuilder, IServiceProvider, TracerProviderBuilder> tracerProviderBuilder)
        {
            AddTracerProvider = tracerProviderBuilder;
        }

        internal void SetOtlpExporterOptions(Action<OtlpExporterDto> act)
        {
            AddExporterOption = options =>
            {
                act(options);
                ServiceCollection.AddSingleton(options);
                return this;
            };
        }
    }
}