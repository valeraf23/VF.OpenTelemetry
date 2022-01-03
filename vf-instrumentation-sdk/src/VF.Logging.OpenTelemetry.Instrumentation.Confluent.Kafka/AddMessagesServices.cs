using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.ActivityTags;
using VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Consumer;
using VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Consumer.Implementation;
using VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Producer;
using VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Producer.Implementation;
using System;
using System.Collections.Generic;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka
{
    public static class AddMessagesServices
    {
        private static IServiceCollection AddKafkaMessageSender(this IServiceCollection services, IEnumerable<string> tags)
            => services
                .AddSingleton(Tags.Create(tags))
                .AddSingleton<IKafkaInjectTrace, KafkaInjectTrace>()
                .AddSingleton<IAddTagsProducer, TagsProducer>()
                .AddSingleton<IMessageSender, KafkaMessageSender>();

        public static IServiceCollection AddOtelKafkaProducer(this IServiceCollection services, ProducerConfig configuration, IEnumerable<string> tags)
            => services
                .AddKafkaMessageSender(tags)
                .AddSingleton(configuration)
                .AddSingleton(typeof(IKafkaProducerBuilder<,>), typeof(DiProducerBuilder<,>))
                .AddSingleton(typeof(IProducer<,>), typeof(KafkaProducerWrapper<,>));

        public static IServiceCollection AddOtelKafkaProducer(this IServiceCollection services, ProducerConfig configuration)
            => services.AddOtelKafkaProducer(configuration, Array.Empty<string>());

        public static IServiceCollection AddOtelKafkaProducer<TKey, TValue>(this IServiceCollection services, IProducer<TKey, TValue> producer, ProducerConfig configuration, IEnumerable<string> tags)
        {
            services.TryAddSingleton(producer);
            services.TryAddSingleton(configuration);
            return services
                .AddKafkaMessageSender(tags)
                .Decorate<IProducer<TKey, TValue>, KafkaProducer<TKey, TValue>>();
        }
        public static IServiceCollection AddOtelKafkaProducer<TKey, TValue>(this IServiceCollection services, IProducer<TKey, TValue> producer, ProducerConfig configuration) => services.AddOtelKafkaProducer(producer, configuration, Array.Empty<string>());

        private static IServiceCollection AddKafkaMessageReceiver(this IServiceCollection services, IEnumerable<string> tags)
            => services
                .AddSingleton<IKafkaExtractTrace, KafkaExtractTrace>()
                .AddSingleton(Tags.Create(tags))
                .AddSingleton<IAddTagsConsumer, MessagingTagsConsumer>()
                .AddSingleton<IMessageReceiver, KafkaMessageReceiver>();

        public static IServiceCollection AddOtelKafkaConsumer(this IServiceCollection services, ConsumerConfig configuration, IEnumerable<string> tags)
            => services
                .AddKafkaMessageReceiver(tags)
                .AddSingleton(configuration)
                .AddSingleton(typeof(IConsumerBuilder<,>), typeof(DiConsumerBuilder<,>))
                .AddSingleton(typeof(IConsumer<,>), typeof(KafkaConsumerWrapper<,>));

        public static IServiceCollection AddOtelKafkaConsumer<TKey, TValue>(this IServiceCollection services, IConsumer<TKey, TValue> consumer, ConsumerConfig configuration, IEnumerable<string> tags)
        {
            services.TryAddSingleton(consumer);
            services.TryAddSingleton(configuration);
            return services.AddKafkaMessageReceiver(tags)
                .Decorate<IConsumer<TKey, TValue>, KafkaConsumer<TKey, TValue>>();
        }
        public static IServiceCollection AddOtelKafkaConsumer(this IServiceCollection services, ConsumerConfig configuration)
            => services.AddOtelKafkaConsumer(configuration, Array.Empty<string>());

        public static IServiceCollection AddOtelKafkaConsumer<TKey, TValue>(this IServiceCollection services, IConsumer<TKey, TValue> consumer, ConsumerConfig configuration) => services.AddOtelKafkaConsumer(consumer, configuration, Array.Empty<string>());
    }
}