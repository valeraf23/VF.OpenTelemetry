namespace VF.Logging.OpenTelemetry.Configuration
{
    public class AspNetCoreConfiguration
    {
        public bool RecordException { get; set; } = true;
        public bool EnableGrpcAspNetCoreSupport { get; set; } = true;
    }
}