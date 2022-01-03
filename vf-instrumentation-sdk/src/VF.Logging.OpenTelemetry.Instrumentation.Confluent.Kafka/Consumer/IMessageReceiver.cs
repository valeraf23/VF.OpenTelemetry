using System;
using Confluent.Kafka;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Consumer
{
    public interface IMessageReceiver
    {
        ConsumeResult<TKey, TValue> ReceiveMessage<TKey, TValue>(Func<ConsumeResult<TKey, TValue>> receiver);
    }
}