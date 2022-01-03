using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;
using VF.Logging.OpenTelemetry.Configuration;

namespace VF.Logging.OpenTelemetry.TraceInstrumentation.Implementation
{
    public class EFCoreInstrumentation : IInstrumentation
    {
        private readonly EFConfiguration _confOptions;

        public EFCoreInstrumentation(IOptions<EFConfiguration> confOptions) => _confOptions = confOptions.Value;
        public TracerProviderBuilder Add(TracerProviderBuilder builder)
        {
            builder
                .AddEntityFrameworkCoreInstrumentation(op =>
                {
                    op.SetDbStatementForText = _confOptions.SetDbStatementForText;
                    op.SetDbStatementForStoredProcedure = _confOptions.SetDbStatementForStoredProcedure;
                });
            return builder;
        }
    }
}