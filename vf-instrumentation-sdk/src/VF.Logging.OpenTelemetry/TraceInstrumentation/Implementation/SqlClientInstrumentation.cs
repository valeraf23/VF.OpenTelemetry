using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;
using VF.Logging.OpenTelemetry.Configuration;

namespace VF.Logging.OpenTelemetry.TraceInstrumentation.Implementation
{
    public class SqlClientInstrumentation : IInstrumentation
    {
        private readonly SqlClientConfiguration _sqlClientConfiguration;

        public SqlClientInstrumentation(IOptions<SqlClientConfiguration> sqlClient) =>
            _sqlClientConfiguration = sqlClient.Value;

        public TracerProviderBuilder Add(TracerProviderBuilder builder)
        {
            builder
                .AddSqlClientInstrumentation(op =>
                {
                    op.SetDbStatementForText = _sqlClientConfiguration.SetDbStatementForText;
                    op.SetDbStatementForStoredProcedure = _sqlClientConfiguration.SetDbStatementForStoredProcedure;
                });
            return builder;
        }
    }
}