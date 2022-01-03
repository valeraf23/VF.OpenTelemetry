using Confluent.Kafka;
using VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.ActivityTags;
using VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Extensions;
using System.Diagnostics;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Producer.Implementation
{
    public class TagsProducer : MessagingTags, IAddTagsProducer
    {
        private readonly ProducerConfig _producerConfig;
        private readonly string[] _values;

        public TagsProducer(ProducerConfig producerConfig, ITags tags)
        {
            _values = tags.Values;
            _producerConfig = producerConfig;
        }

        public void AddMessagingTags<TKey, TValue>(Activity? activity, Message<TKey, TValue> message)
        {
            foreach (var tag in _values)
                switch (tag)
                {
                    case Tags.Body:
                        activity?.SetTag(Tags.Body, message.Value);
                        break;
                    case null:
                        break;
                    default:
                        message.Headers.TryGetValue(tag, out var header);
                        activity?.SetTag(tag, header);
                        break;
                }

            if (_producerConfig.Partitioner is not null)
            {
                activity?.SetTag("messaging.kafka.partition", _producerConfig.Partitioner.ToString());
            }

            AddMessagingTags(activity, _producerConfig.BootstrapServers, _producerConfig.ClientId);
        }

        private static class Tags
        {
            public const string Body = "body";
        }
    }
}