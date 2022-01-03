using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;
using VF.Logging.OpenTelemetry.Configuration;

namespace VF.Logging.OpenTelemetry.TraceInstrumentation.Implementation
{
    public class AspNetCoreInstrumentation : IInstrumentation
    {
        private readonly AspNetCoreConfiguration _aspNetCoreConfiguration;

        public AspNetCoreInstrumentation(IOptions<AspNetCoreConfiguration> aspNetCore)
        {
            _aspNetCoreConfiguration = aspNetCore.Value;
        }

        public TracerProviderBuilder Add(TracerProviderBuilder builder)
        {
            builder.AddAspNetCoreInstrumentation(x =>
            {
#if NETSTANDARD2_1
                    x.EnableGrpcAspNetCoreSupport = _aspNetCoreConfiguration.EnableGrpcAspNetCoreSupport;
#endif
                x.RecordException = _aspNetCoreConfiguration.RecordException;
            });
            return builder;
        }
    }
}