using OpenTelemetry.Trace;
using VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Extensions;
using VF.Logging.OpenTelemetry.TraceInstrumentation;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka
{
    public class KafkaInstrumentation : IInstrumentation
    {
        public TracerProviderBuilder Add(TracerProviderBuilder builder) => builder.AddKafkaInstrumentation();
    }
}