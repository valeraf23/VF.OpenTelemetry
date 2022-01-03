using System;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Producer
{
    public interface IMessageSender
    {
        Task<DeliveryResult<TKey, TValue>> SendMessage<TKey, TValue>(Message<TKey, TValue> message, Func<Message<TKey, TValue>, Task<DeliveryResult<TKey, TValue>>> sender);

        void SendMessage<TKey, TValue>(Message<TKey, TValue> message, Action<Message<TKey, TValue>> sender);
    }
}