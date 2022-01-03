using System.Diagnostics;
using Confluent.Kafka;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Consumer
{
    public interface IAddTagsConsumer
    {
        void AddMessagingTags<TKey, TValue>(Activity? activity, ConsumeResult<TKey, TValue> message);
    }
}