using System;
using System.Collections.Generic;
using System.Threading;
using Confluent.Kafka;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Consumer.Implementation
{
    public class KafkaConsumer<TKey, TValue> : IConsumer<TKey, TValue>
    {
        private readonly IConsumer<TKey, TValue> _consumer;
        private readonly IMessageReceiver _messageReceiver;

        public KafkaConsumer(IConsumer<TKey, TValue> consumer, IMessageReceiver messageReceiver)
        {
            _messageReceiver = messageReceiver;
            _consumer = consumer;
            MemberId = _consumer.MemberId;
            Assignment = _consumer.Assignment;
            Subscription = _consumer.Subscription;
            ConsumerGroupMetadata = _consumer.ConsumerGroupMetadata;
            Handle = _consumer.Handle;
            Name = _consumer.Name;
        }

        public void Dispose() => _consumer.Dispose();

        public int AddBrokers(string brokers) => _consumer.AddBrokers(brokers);

        public Handle Handle { get; }
        public string Name { get; }

        public ConsumeResult<TKey, TValue> Consume(CancellationToken cancellationToken = default) => Consume(Receive(cancellationToken));

        public ConsumeResult<TKey, TValue> Consume(TimeSpan timeout) => Consume(Receive(timeout));

        public ConsumeResult<TKey, TValue> Consume(int millisecondsTimeout) => Consume(Receive(millisecondsTimeout));

        public void Subscribe(IEnumerable<string> topics) => _consumer.Subscribe(topics);

        public void Subscribe(string topic) => _consumer.Subscribe(topic);

        public void Unsubscribe() => _consumer.Unsubscribe();

        public void Assign(TopicPartition partition) => _consumer.Assign(partition);

        public void Assign(TopicPartitionOffset partition) => _consumer.Assign(partition);

        public void Assign(IEnumerable<TopicPartitionOffset> partitions) => _consumer.Assign(partitions);

        public void Assign(IEnumerable<TopicPartition> partitions) => _consumer.Assign(partitions);

        public void IncrementalAssign(IEnumerable<TopicPartitionOffset> partitions) => _consumer.IncrementalAssign(partitions);

        public void IncrementalAssign(IEnumerable<TopicPartition> partitions) => _consumer.IncrementalAssign(partitions);

        public void IncrementalUnassign(IEnumerable<TopicPartition> partitions) => _consumer.IncrementalUnassign(partitions);

        public void Unassign() => _consumer.Unassign();

        public void StoreOffset(ConsumeResult<TKey, TValue> result) => _consumer.StoreOffset(result);

        public void StoreOffset(TopicPartitionOffset offset) => _consumer.StoreOffset(offset);

        public List<TopicPartitionOffset> Commit() => _consumer.Commit();

        public void Commit(IEnumerable<TopicPartitionOffset> offsets) => _consumer.Commit(offsets);

        public void Commit(ConsumeResult<TKey, TValue> result) => _consumer.Commit(result);

        public void Seek(TopicPartitionOffset tpo) => _consumer.Seek(tpo);

        public void Pause(IEnumerable<TopicPartition> partitions) => _consumer.Pause(partitions);

        public void Resume(IEnumerable<TopicPartition> partitions) => _consumer.Resume(partitions);

        public List<TopicPartitionOffset> Committed(TimeSpan timeout) => _consumer.Committed(timeout);

        public List<TopicPartitionOffset> Committed(IEnumerable<TopicPartition> partitions, TimeSpan timeout) => _consumer.Committed(partitions, timeout);

        public Offset Position(TopicPartition partition) => _consumer.Position(partition);

        public List<TopicPartitionOffset> OffsetsForTimes(IEnumerable<TopicPartitionTimestamp> timestampsToSearch,
            TimeSpan timeout) => _consumer.OffsetsForTimes(timestampsToSearch, timeout);

        public WatermarkOffsets GetWatermarkOffsets(TopicPartition topicPartition) => _consumer.GetWatermarkOffsets(topicPartition);

        public WatermarkOffsets QueryWatermarkOffsets(TopicPartition topicPartition, TimeSpan timeout) => _consumer.QueryWatermarkOffsets(topicPartition, timeout);

        public void Close() => _consumer.Close();

        public string MemberId { get; }
        public List<TopicPartition> Assignment { get; }
        public List<string> Subscription { get; }
        public IConsumerGroupMetadata ConsumerGroupMetadata { get; }

        private ConsumeResult<TKey, TValue> Consume(Func<ConsumeResult<TKey, TValue>> receiver) => _messageReceiver.ReceiveMessage(receiver);

        private Func<ConsumeResult<TKey, TValue>> Receive(TimeSpan timeout)
        {
            return () =>
            {
                var cr = _consumer.Consume(timeout);
                return cr;
            };
        }

        private Func<ConsumeResult<TKey, TValue>> Receive(int millisecondsTimeout)
        {
            return () =>
            {
                var cr = _consumer.Consume(millisecondsTimeout);
                return cr;
            };
        }

        private Func<ConsumeResult<TKey, TValue>> Receive(CancellationToken cancellationToken)
        {
            return () =>
            {
                var cr = _consumer.Consume(cancellationToken);
                return cr;
            };
        }
    }
}