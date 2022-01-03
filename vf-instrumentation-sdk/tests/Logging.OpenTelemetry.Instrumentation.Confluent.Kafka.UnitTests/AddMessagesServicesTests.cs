using System;
using Confluent.Kafka;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.UnitTests
{
    public abstract class AddMessagesServicesTests
    {
        protected readonly ServiceCollection Services;

        protected AddMessagesServicesTests()
        {
            Services = new ServiceCollection();
            Services.AddLogging();
        }
    }

    public abstract class AddProducerServicesTests<TKey, TValue> : AddMessagesServicesTests
    {
        [Fact]
        public void AddOtelKafkaProducer()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            var producerConfig = new ProducerConfig();
            services.AddOtelKafkaProducer(producerConfig);
            var provider = services.BuildServiceProvider();
            var producer = provider.GetService<IProducer<TKey, TValue>>();
            producer.Should().NotBeNull();
        }
    }

    public abstract class AddConsumerServicesTests<TKey, TValue> : AddMessagesServicesTests
    {
        [Fact]
        public void AddOtelKafkaConsumer()
        {
            var consumerConfig = new ConsumerConfig
            {
                GroupId = "consumer-group"
            };
            Services.AddOtelKafkaConsumer(consumerConfig, Array.Empty<string>());
            var provider = Services.BuildServiceProvider();
            var consumer = provider.GetService<IConsumer<TKey, TValue>>();
            consumer.Should().NotBeNull();
        }
    }

    public class String_String_ProducerTest : AddProducerServicesTests<string, string>
    {
    }

    public class Null_Int_ProducerTest : AddProducerServicesTests<Null, int>
    {
    }

    public class Int_String_ProducerTest : AddProducerServicesTests<int, string>
    {
    }

    public class String_String_ConsumerTest : AddConsumerServicesTests<string, string>
    {
    }

    public class Ignore_Int_ConsumerTest : AddConsumerServicesTests<Ignore, int>
    {
    }

    public class Int_String_ConsumerTest : AddConsumerServicesTests<int, string>
    {
    }
}