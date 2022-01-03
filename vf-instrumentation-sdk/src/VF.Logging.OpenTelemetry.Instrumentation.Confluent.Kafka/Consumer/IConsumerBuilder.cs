using Confluent.Kafka;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Consumer
{
    public interface IConsumerBuilder<TKey, TValue>
    {
        IConsumer<TKey, TValue> Build();
    }
}