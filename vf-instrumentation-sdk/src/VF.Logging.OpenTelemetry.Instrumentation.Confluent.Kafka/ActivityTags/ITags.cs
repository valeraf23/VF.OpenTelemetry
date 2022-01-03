namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.ActivityTags
{
    public interface ITags
    {
        string[] Values { get; set; }
    }
}