namespace VF.Logging.OpenTelemetry.Configuration
{
    public class SqlClientConfiguration
    {
        public bool SetDbStatementForText { get; set; } = true;
        public bool SetDbStatementForStoredProcedure { get; set; } = true;
    }
}