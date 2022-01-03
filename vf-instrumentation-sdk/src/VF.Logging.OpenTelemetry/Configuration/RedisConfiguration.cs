namespace VF.Logging.OpenTelemetry.Configuration
{
    public class RedisConfiguration
    {
        public FlushInterval FlushInterval { get; set; } = new();
    }

    public class FlushInterval
    {
        public string Format { get; set; } = "%s";
        public int Interval { get; set; } = 5;
    }
}