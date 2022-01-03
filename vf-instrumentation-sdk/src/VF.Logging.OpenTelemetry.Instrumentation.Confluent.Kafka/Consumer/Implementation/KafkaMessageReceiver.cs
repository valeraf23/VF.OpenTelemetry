using System;
using System.Diagnostics;
using Confluent.Kafka;
using VF.Logging.OpenTelemetry.MapPropagator;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Consumer.Implementation
{
    public class KafkaMessageReceiver : IMessageReceiver
    {
        private static readonly ActivitySource ActivitySource = new(nameof(KafkaMessageReceiver));
        private readonly IKafkaExtractTrace _kafkaExtractTrace;
        private readonly IAddTagsConsumer _addTagsConsumer;

        public KafkaMessageReceiver(IKafkaExtractTrace kafkaExtractTrace, IAddTagsConsumer addTagsConsumer)
        {
            _kafkaExtractTrace = kafkaExtractTrace;
            _addTagsConsumer = addTagsConsumer;
        }

        public ConsumeResult<TKey, TValue> ReceiveMessage<TKey, TValue>(Func<ConsumeResult<TKey, TValue>> receiver)
            => MessagingMapPropagator.ReceiveMessage(
                ActivitySource,
                _kafkaExtractTrace.ExtractTraceContextFromBasicProperties,
                receiver,
                "kafka receive",
                _addTagsConsumer.AddMessagingTags);
    }
}