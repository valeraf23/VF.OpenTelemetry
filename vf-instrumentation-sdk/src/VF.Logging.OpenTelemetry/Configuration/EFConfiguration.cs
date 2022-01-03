namespace VF.Logging.OpenTelemetry.Configuration
{
    public class EFConfiguration
    {
        public bool SetDbStatementForText { get; set; } = true;
        public bool SetDbStatementForStoredProcedure { get; set; } = true;
    }
}