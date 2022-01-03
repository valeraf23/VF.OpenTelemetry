using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using VF.Logging.OpenTelemetry.MapPropagator;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Producer.Implementation
{
    public class KafkaMessageSender : IMessageSender
    {
        private static readonly ActivitySource ActivitySource = new(nameof(KafkaMessageSender));
        private readonly IAddTagsProducer _addTagsProducer;
        private readonly IKafkaInjectTrace _kafkaInjectTrace;

        private readonly ILogger<KafkaMessageSender> _logger;

        public KafkaMessageSender(ILogger<KafkaMessageSender> logger, IAddTagsProducer addTagsProducer, IKafkaInjectTrace kafkaInjectTrace)
        {
            _logger = logger;
            _addTagsProducer = addTagsProducer;
            _kafkaInjectTrace = kafkaInjectTrace;
        }

        public async Task<DeliveryResult<TKey, TValue>> SendMessage<TKey, TValue>(Message<TKey, TValue> message, Func<Message<TKey, TValue>, Task<DeliveryResult<TKey, TValue>>> sender)
        {
            try
            {
                return await MessagingMapPropagator.SendMessage(
                    ActivitySource,
                    message,
                    _kafkaInjectTrace.InjectTraceContextIntoBasicProperties, sender, "kafka send",
                    _addTagsProducer.AddMessagingTags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Message publishing failed.", Array.Empty<object>());
                throw;
            }
        }

        public void SendMessage<TKey, TValue>(Message<TKey, TValue> message, Action<Message<TKey, TValue>> sender)
        {
            try
            {
                MessagingMapPropagator.SendMessage(
                    ActivitySource, 
                    message, _kafkaInjectTrace.InjectTraceContextIntoBasicProperties, sender,
                    "kafka send", _addTagsProducer.AddMessagingTags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Message publishing failed.", Array.Empty<object>());
                throw;
            }
        }
    }
}