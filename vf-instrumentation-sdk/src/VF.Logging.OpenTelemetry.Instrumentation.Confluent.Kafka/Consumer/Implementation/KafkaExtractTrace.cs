using System;
using System.Collections.Generic;
using System.Linq;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Extensions;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Consumer.Implementation
{
    public class KafkaExtractTrace : IKafkaExtractTrace
    {
        private readonly ILogger<KafkaExtractTrace> _logger;

        public KafkaExtractTrace(ILogger<KafkaExtractTrace> logger)
        {
            _logger = logger;
        }

        public IEnumerable<string> ExtractTraceContextFromBasicProperties<TKey, TValue>(ConsumeResult<TKey, TValue> props, string key)
        {
            try
            {
                if (props.Message.Headers.TryGetValue(key, out var value)) return new[] {value};
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to extract trace context: {ex}", Array.Empty<object>());
            }

            return Enumerable.Empty<string>();
        }
    }
}