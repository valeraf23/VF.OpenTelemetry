using System.Collections.Generic;
using Confluent.Kafka;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Consumer
{
    public interface IKafkaExtractTrace
    {
        IEnumerable<string> ExtractTraceContextFromBasicProperties<TKey, TValue>(ConsumeResult<TKey, TValue> props, string key);
    }
}