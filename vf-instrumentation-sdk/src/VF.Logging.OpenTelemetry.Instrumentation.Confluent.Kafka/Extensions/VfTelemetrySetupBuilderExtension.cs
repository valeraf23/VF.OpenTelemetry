using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Extensions
{
    public static class VfTelemetrySetupBuilderExtension
    {
        private static OpenTelemetrySetupBuilder AddKafkaInstrumentation(this OpenTelemetrySetupBuilder builder) => builder
                .AddInstrumentation(Assembly.GetAssembly(typeof(KafkaInstrumentation))!, Array.Empty<Assembly>());

        public static OpenTelemetrySetupBuilder AddOtelKafkaProducer<TKey, TValue>(
            this OpenTelemetrySetupBuilder builder, IProducer<TKey, TValue> producer, ProducerConfig configuration,
            IEnumerable<string> tags) => builder
                .AddKafkaInstrumentation()
                .AddService(x => x.AddOtelKafkaProducer(producer, configuration, tags));

        public static OpenTelemetrySetupBuilder AddOtelKafkaProducer<TKey, TValue>(
            this OpenTelemetrySetupBuilder builder, IProducer<TKey, TValue> producer, ProducerConfig configuration) => builder.AddOtelKafkaProducer(producer, configuration, Array.Empty<string>());

        public static OpenTelemetrySetupBuilder AddOtelKafkaConsumer<TKey, TValue>(
            this OpenTelemetrySetupBuilder builder, IConsumer<TKey, TValue> consumer, ConsumerConfig configuration,
            IEnumerable<string> tags) => builder
                .AddKafkaInstrumentation()
                .AddService(x => x.AddOtelKafkaConsumer(consumer, configuration, tags));

        public static OpenTelemetrySetupBuilder AddOtelKafkaConsumer<TKey, TValue>(
            this OpenTelemetrySetupBuilder builder, IConsumer<TKey, TValue> consumer, ConsumerConfig configuration) => builder.AddOtelKafkaConsumer(consumer, configuration, Array.Empty<string>());

        public static OpenTelemetrySetupBuilder AddOtelKafkaProducer(this OpenTelemetrySetupBuilder builder,
            ProducerConfig config, IEnumerable<string> tags) => builder
                .AddKafkaInstrumentation()
                .AddService(x => x.AddOtelKafkaProducer(config, tags));

        public static OpenTelemetrySetupBuilder AddOtelKafkaProducer(this OpenTelemetrySetupBuilder builder,
            ProducerConfig config) => builder
                .AddOtelKafkaProducer(config, Array.Empty<string>());

        public static OpenTelemetrySetupBuilder AddOtelKafkaConsumer(this OpenTelemetrySetupBuilder builder,
            ConsumerConfig config, IEnumerable<string> tags) => builder
                .AddKafkaInstrumentation()
                .AddService(x => x.AddOtelKafkaConsumer(config, tags));

        public static OpenTelemetrySetupBuilder AddOtelKafkaConsumer(this OpenTelemetrySetupBuilder builder,
            ConsumerConfig config) => builder
                .AddOtelKafkaConsumer(config, Array.Empty<string>());
    }
}