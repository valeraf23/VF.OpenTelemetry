using OpenTelemetry.Trace;
using VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Consumer.Implementation;
using VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Producer.Implementation;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Extensions
{
    public static class KafkaTracerProviderBuilderExtension
    {
        public static TracerProviderBuilder AddKafkaInstrumentation(this TracerProviderBuilder builder)
            => builder
                .AddSource(nameof(KafkaMessageSender))
                .AddSource(nameof(KafkaMessageReceiver));
    }
}