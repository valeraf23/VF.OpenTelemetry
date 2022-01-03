using AutoFixture.Xunit2;
using Confluent.Kafka;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Extensions;
using VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Producer.Implementation;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.UnitTests
{
    public class KafkaProducerTests
    {
        private readonly ServiceCollection _services;

        public KafkaProducerTests()
        {
            _services = new ServiceCollection();
            _services.AddLogging();
            AddActivityListener();
        }

        private List<string?> ActualTraceIds { get; } = new();

        private List<KeyValuePair<string, string?>>? ActualTags { get; } = new();

        [Fact]
        public async Task ShouldPropagateParentId()
        {
            // arrange
            const string? testName = nameof(ShouldPropagateParentId);
            var settings = new Dictionary<string, string> { { "VFTelemetry", "" } };
            var producerConfig = new ProducerConfig();
            var message = new Message<Null, string> { Value = "Tests" };
            var producerMock = new Mock<IProducer<Null, string>>();
            producerMock.Setup(x => x.ProduceAsync(It.IsAny<string>(), message, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new DeliveryResult<Null, string>())).Verifiable();

            // act
            using ActivitySource activitySource = new(nameof(ShouldPropagateParentId));
            var configurationBuilder = new ConfigurationBuilder().AddInMemoryCollection(settings);
            var configuration = configurationBuilder.Build();
            _services.AddOpenTelemetry(configuration, testName, builder => builder.AddOtelKafkaProducer(producerMock.Object, producerConfig));
            var provider = _services.BuildServiceProvider();
            var consumer = provider.GetRequiredService<IProducer<Null, string>>();
            await consumer.ProduceAsync(It.IsAny<string>(), message, It.IsAny<CancellationToken>());

            // assert
            producerMock.Verify(mock => mock.ProduceAsync(It.IsAny<string>(), message, It.IsAny<CancellationToken>()),
                Times.Once());
            message.Headers.Should().Contain(x =>
                x.Key.Equals("traceparent") && ActualTraceIds.Contains(Encoding.ASCII.GetString(x.GetValueBytes())));
        }

        [Theory]
        [AutoData]
        public async Task ShouldAddTags(string testBody)
        {
            // arrange
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = "localhost",
                ClientId = "clientId.id",
                Partitioner = Partitioner.Consistent
            };
            const string? testName = nameof(ShouldAddTags);

            var expectedTags = new Dictionary<string, string>
            {
                ["messaging.kafka.partition"] = producerConfig.Partitioner.ToString()!,
                ["messaging.url"] = producerConfig.BootstrapServers,
                ["messaging.kafka.client_id"] = producerConfig.ClientId,
                ["messaging.system"] = "kafka",
                ["messaging.destination_kind"] = "topic"
            };

            var message = new Message<Null, string> { Value = testBody };
            var producerMock = new Mock<IProducer<Null, string>>();
            producerMock.Setup(x => x.ProduceAsync(It.IsAny<string>(), message, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new DeliveryResult<Null, string>()));

            // act
            using ActivitySource activitySource = new(testName);
            _services.AddOtelKafkaProducer(producerMock.Object, producerConfig);
            var provider = _services.BuildServiceProvider();
            var consumer = provider.GetRequiredService<IProducer<Null, string>>();
            await consumer.ProduceAsync(It.IsAny<string>(), message, It.IsAny<CancellationToken>());

            // assert

            ActualTags.Should().NotBeNull();
            ActualTags.Should().Contain(expectedTags!);
        }

        private void AddActivityListener()
        {
            var activityListener = new ActivityListener
            {
                ShouldListenTo = x => x.Name == nameof(KafkaMessageSender),
                SampleUsingParentId = (ref ActivityCreationOptions<string> _) => ActivitySamplingResult.AllData,
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
                ActivityStopped = activity =>
                {
                    ActualTraceIds.Add(activity.Id);
                    ActualTags.AddRange(activity.Tags);
                }
            };
            ActivitySource.AddActivityListener(activityListener);
        }
    }
}