using Confluent.Kafka;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Producer
{
    public interface IKafkaInjectTrace
    {
        void InjectTraceContextIntoBasicProperties<TKey, TValue>(Message<TKey, TValue> props, string key, string value);
    }
}