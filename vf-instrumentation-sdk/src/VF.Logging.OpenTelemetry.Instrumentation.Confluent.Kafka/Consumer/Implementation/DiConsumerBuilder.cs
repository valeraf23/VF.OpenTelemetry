using System;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Consumer.Implementation
{
    public class DiConsumerBuilder<TKey, TValue> : IConsumerBuilder<TKey, TValue>
    {
        private readonly ILogger<KafkaConsumerWrapper<TKey, TValue>> _logger;
        private readonly ConsumerConfig _consumerConfig;

        public DiConsumerBuilder(ILogger<KafkaConsumerWrapper<TKey, TValue>> logger, ConsumerConfig consumerConfig)
        {
            _logger = logger;
            _consumerConfig = consumerConfig;

        }

        public IConsumer<TKey, TValue> Build()
        {
            if (Activator.CreateInstance(typeof(ConsumerBuilder<TKey, TValue>), _consumerConfig) is
                ConsumerBuilder<TKey, TValue> consumerBuilder) return consumerBuilder.Build();
            var errorMsg = $"ConsumerBuilder of type {typeof(ConsumerBuilder<TKey, TValue>)} was not created";
            _logger.LogCritical(errorMsg, Array.Empty<object>());
            throw new Exception(errorMsg);
        }
    }
}