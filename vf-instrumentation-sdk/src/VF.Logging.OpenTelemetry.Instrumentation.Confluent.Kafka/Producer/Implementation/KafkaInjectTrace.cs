using System;
using System.Text;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Producer.Implementation
{
    public class KafkaInjectTrace : IKafkaInjectTrace
    {
        private readonly ILogger<KafkaInjectTrace> _logger;

        public KafkaInjectTrace(ILogger<KafkaInjectTrace> logger)
        {
            _logger = logger;
        }

        public void InjectTraceContextIntoBasicProperties<TKey, TValue>(Message<TKey, TValue> props, string key, string value)
        {
            try
            {
                props.Headers ??= new Headers();
                props.Headers.Add(key, Encoding.UTF8.GetBytes(value));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to inject trace context.", Array.Empty<object>());
            }
        }
    }
}