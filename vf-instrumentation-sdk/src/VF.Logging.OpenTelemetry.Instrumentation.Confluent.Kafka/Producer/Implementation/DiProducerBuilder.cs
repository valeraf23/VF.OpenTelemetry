using System;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Producer.Implementation
{
    public class DiProducerBuilder<TKey, TValue> : IKafkaProducerBuilder<TKey, TValue>
    {
        private readonly ILogger<KafkaProducerWrapper<TKey, TValue>> _logger;
        private readonly ProducerConfig _producerConfig;

        public DiProducerBuilder(ILogger<KafkaProducerWrapper<TKey, TValue>> logger, ProducerConfig producerConfig)
        {
            _logger = logger;
            _producerConfig = producerConfig;
        }

        public IProducer<TKey, TValue> Build()
        {
            if (Activator.CreateInstance(typeof(ProducerBuilder<TKey, TValue>), _producerConfig) is
                ProducerBuilder<TKey, TValue> producerBuilder) return producerBuilder.Build();
            var errorMsg = $"ProducerBuilder of type {typeof(ProducerBuilder<TKey, TValue>)} was not created";
            _logger.LogCritical(errorMsg,Array.Empty<object>());
            throw new Exception(errorMsg);
        }
    }
}