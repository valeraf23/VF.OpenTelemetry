using Confluent.Kafka;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Producer
{
    public interface IKafkaProducerBuilder<TKey, TValue>
    {
        IProducer<TKey, TValue> Build();
    }
}