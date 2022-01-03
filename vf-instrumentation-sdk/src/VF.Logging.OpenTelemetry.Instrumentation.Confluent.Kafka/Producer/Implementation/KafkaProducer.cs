using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Producer.Implementation
{
    public class KafkaProducer<TKey, TValue> : IProducer<TKey, TValue>
    {
        private readonly IMessageSender _messageSender;
        private readonly IProducer<TKey, TValue> _producer;

        public KafkaProducer(IProducer<TKey, TValue> producer, IMessageSender messageSender)
        {
            _messageSender = messageSender;
            _producer = producer;
            Handle = _producer.Handle;
            Name = _producer.Name;
        }

        public void Dispose() => _producer.Dispose();

        public int AddBrokers(string brokers) => _producer.AddBrokers(brokers);

        public Handle Handle { get; }
        public string Name { get; }

        public async Task<DeliveryResult<TKey, TValue>> ProduceAsync(string topic, Message<TKey, TValue> message, CancellationToken cancellationToken = new())
            => await ProduceAsync(message, msg => _producer.ProduceAsync(topic, msg, cancellationToken));

        public async Task<DeliveryResult<TKey, TValue>> ProduceAsync(TopicPartition topicPartition, Message<TKey, TValue> message, CancellationToken cancellationToken = new())
            => await ProduceAsync(message, msg => _producer.ProduceAsync(topicPartition, msg, cancellationToken));

        public void Produce(string topic, Message<TKey, TValue> message, Action<DeliveryReport<TKey, TValue>>? deliveryHandler = null)
            => Produce(message, msg => _producer.Produce(topic, msg, deliveryHandler));

        public void Produce(TopicPartition topicPartition, Message<TKey, TValue> message, Action<DeliveryReport<TKey, TValue>>? deliveryHandler = null)
            => Produce(message, msg => _producer.Produce(topicPartition, msg, deliveryHandler));

        public int Poll(TimeSpan timeout) => _producer.Poll(timeout);

        public int Flush(TimeSpan timeout) => _producer.Flush(timeout);

        public void Flush(CancellationToken cancellationToken = new()) => _producer.Flush(cancellationToken);

        public void InitTransactions(TimeSpan timeout) => _producer.InitTransactions(timeout);

        public void BeginTransaction() => _producer.BeginTransaction();

        public void CommitTransaction(TimeSpan timeout) => _producer.CommitTransaction(timeout);

        public void CommitTransaction() => _producer.CommitTransaction();

        public void AbortTransaction(TimeSpan timeout) => _producer.AbortTransaction(timeout);

        public void AbortTransaction() => _producer.AbortTransaction();

        public void SendOffsetsToTransaction(IEnumerable<TopicPartitionOffset> offsets, IConsumerGroupMetadata groupMetadata, TimeSpan timeout)
            => _producer.SendOffsetsToTransaction(offsets, groupMetadata, timeout);

        private async Task<DeliveryResult<TKey, TValue>> ProduceAsync(Message<TKey, TValue> message, Func<Message<TKey, TValue>, Task<DeliveryResult<TKey, TValue>>> sender)
            => await _messageSender.SendMessage(message, sender);

        private void Produce(Message<TKey, TValue> message, Action<Message<TKey, TValue>> sender)
            => _messageSender.SendMessage(message, sender);
    }
}