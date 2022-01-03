using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;
using VF.Logging.OpenTelemetry.TraceInstrumentation;

namespace VF.Logging.OpenTelemetry.Extensions.TracerProviderBuilderExtensions
{
    internal static class VfInstrumentationTracerProviderBuilderExtensions
    {
        internal static TracerProviderBuilder AddTraceInstrumentation(this TracerProviderBuilder builder,
            IServiceProvider serviceProvider)
        {
            var implementations = serviceProvider.GetService<IEnumerable<IInstrumentation>>()?.ToArray();

            if (implementations is null || !implementations.Any()) return builder;
            foreach (var instrument in implementations)
            {
                instrument.Add(builder);
            }

            return builder;
        }
    }
}