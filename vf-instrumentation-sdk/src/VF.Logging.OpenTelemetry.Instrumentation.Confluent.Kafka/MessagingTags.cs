using System.Diagnostics;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka
{
    public class MessagingTags
    {
        protected static void AddMessagingTags(Activity? activity, string bootstrapServers, string clientId)
        {
            activity?.SetTag("messaging.system", "kafka");
            activity?.SetTag("messaging.url", bootstrapServers);
            activity?.SetTag("messaging.destination_kind", "topic");
            activity?.SetTag("messaging.kafka.client_id", clientId);
        }
    }
}