using System;

namespace KafkaConsoleApp
{
    internal class Configurations
    {
        public Configurations()
        {
            Assign();
        }

        public string producer_topic { get; private set; } = "console_topic_test";
        public string consumer_topic { get; private set; } = "console_topic_test";
        public string kafka_endpoint { get; private set; } = "localhost:29092";
        public string otlp_exporter { get; private set; } = "http://localhost:4317/";

        private void Assign()
        {
            var providerTopic = Environment.GetEnvironmentVariable(nameof(producer_topic));
            if (!string.IsNullOrEmpty(providerTopic))
            {
                producer_topic = providerTopic;
            }

            var consumerTopic = Environment.GetEnvironmentVariable(nameof(consumer_topic));
            if (!string.IsNullOrEmpty(consumerTopic))
            {
                consumer_topic = consumerTopic;
            }

            var kafkaEndpoint = Environment.GetEnvironmentVariable(nameof(kafka_endpoint));
            if (!string.IsNullOrEmpty(kafkaEndpoint))
            {
                kafka_endpoint = kafkaEndpoint;
            }

            var otlpExporter = Environment.GetEnvironmentVariable(nameof(otlp_exporter));
            if (!string.IsNullOrEmpty(otlp_exporter))
            {
                otlp_exporter = otlpExporter;
            }
        }
    }
}