using System.Diagnostics;
using Confluent.Kafka;
using VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.ActivityTags;
using VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Extensions;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Consumer.Implementation
{
    public class MessagingTagsConsumer : MessagingTags, IAddTagsConsumer
    {
        private readonly ConsumerConfig _consumerConfig;
        private readonly string[] _values;

        public MessagingTagsConsumer(ConsumerConfig consumerConfig, ITags tags)
        {
            _values = tags.Values;
            _consumerConfig = consumerConfig;
        }

        public void AddMessagingTags<TKey, TValue>(Activity? activity, ConsumeResult<TKey, TValue> msg)
        {
            foreach (var tag in _values)
                switch (tag)
                {
                    case Tags.Body:
                        activity?.SetTag(Tags.Body, msg.Message.Value);
                        break;
                    case Tags.MessageKey:
                        activity?.SetTag(Tags.MessageKey, msg.Message.Key?.ToString());
                        break;
                    case null:
                        break;
                    default:
                        msg.Message.Headers.TryGetValue(tag, out var header);
                        activity?.SetTag(tag, header);
                        break;
                }

            activity?.SetTag("messaging.kafka.consumer_group", _consumerConfig.GroupId);
            AddMessagingTags(activity, _consumerConfig.BootstrapServers, _consumerConfig.ClientId);
        }

        private static class Tags
        {
            public const string Body = "body";
            public const string MessageKey = "messaging.kafka.message_key";
        }
    }
}