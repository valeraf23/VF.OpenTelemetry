using System.Diagnostics;
using Confluent.Kafka;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Producer
{
    public interface IAddTagsProducer
    {
        void AddMessagingTags<TKey, TValue>(Activity? activity, Message<TKey, TValue> message);
    }
}