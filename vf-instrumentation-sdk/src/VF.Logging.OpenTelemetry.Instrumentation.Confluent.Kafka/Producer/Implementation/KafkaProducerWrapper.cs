namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Producer.Implementation
{
    public class KafkaProducerWrapper<TKey, TValue> : KafkaProducer<TKey, TValue>
    {
        public KafkaProducerWrapper(IKafkaProducerBuilder<TKey, TValue> kafkaProducerBuilder, IMessageSender messageSender):base(kafkaProducerBuilder.Build(), messageSender)
        {
        }
    }
}