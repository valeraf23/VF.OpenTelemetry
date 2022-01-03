using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.ActivityTags;
using VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Consumer.Implementation;
using VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Producer.Implementation;
using System;
using System.Collections.Generic;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka
{
    public static class LoggingKafkaSdk
    {
        public static IProducer<TKey, TValue> CreateProducer<TKey, TValue>(ProducerConfig config, ILoggerFactory loggerFactory, IEnumerable<string> tags)
        {
            var producer = new ProducerBuilder<TKey, TValue>(config).Build();
            return CreateProducer(config, loggerFactory, producer, tags);
        }

        public static IProducer<TKey, TValue> CreateProducer<TKey, TValue>(ProducerConfig config, ILoggerFactory loggerFactory, IProducer<TKey, TValue> producer, IEnumerable<string> tags)
        {
            var kafkaInjectTrace = new KafkaInjectTrace(loggerFactory.CreateLogger<KafkaInjectTrace>());
            var messageSender = new KafkaMessageSender(
                loggerFactory.CreateLogger<KafkaMessageSender>(),
                new TagsProducer(config, Tags.Create(tags)), kafkaInjectTrace);
            return new KafkaProducer<TKey, TValue>(producer, messageSender);
        }

        public static IConsumer<TKey, TValue> CreateConsumer<TKey, TValue>(ConsumerConfig config, ILoggerFactory loggerFactory, IEnumerable<string> tags)
        {
            var consumer = new ConsumerBuilder<TKey, TValue>(config);
            return CreateConsumer(config, consumer.Build(), loggerFactory, tags);
        }

        public static IConsumer<TKey, TValue> CreateConsumer<TKey, TValue>(ConsumerConfig config, IConsumer<TKey, TValue> consumer, ILoggerFactory loggerFactory, IEnumerable<string> tags)
        {
            var kafkaExtractTrace = new KafkaExtractTrace(loggerFactory.CreateLogger<KafkaExtractTrace>());
            var addKafkaMessagingTagReceiver = new MessagingTagsConsumer(config, Tags.Create(tags));
            var messageMessageReceiver = new KafkaMessageReceiver(kafkaExtractTrace, addKafkaMessagingTagReceiver);

            return new KafkaConsumer<TKey, TValue>(consumer, messageMessageReceiver);
        }

        public static IConsumer<TKey, TValue> CreateConsumer<TKey, TValue>(ConsumerConfig config, ILoggerFactory loggerFactory)
            => CreateConsumer<TKey, TValue>(config, loggerFactory, Array.Empty<string>());
    }
}