namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Consumer.Implementation
{
    public class KafkaConsumerWrapper<TKey, TValue> : KafkaConsumer<TKey, TValue>
    {
        public KafkaConsumerWrapper(IConsumerBuilder<TKey, TValue> consumerBuilder, IMessageReceiver messageReceiver) : base(consumerBuilder.Build(), messageReceiver)
        {
        }

    }
}