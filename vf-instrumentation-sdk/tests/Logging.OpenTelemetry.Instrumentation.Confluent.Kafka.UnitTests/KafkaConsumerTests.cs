using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using AutoFixture.Xunit2;
using Confluent.Kafka;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Consumer.Implementation;
using VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Extensions;
using Xunit;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.UnitTests
{
    public class KafkaConsumerTests
    {
        private readonly ServiceCollection _services;

        public KafkaConsumerTests()
        {
            _services = new ServiceCollection();
            _services.AddLogging();
            AddActivityListener();
        }

        private List<string?> ActualTraceIds { get; } = new();

        private IEnumerable<KeyValuePair<string, string?>>? ActualTags { get; set; }

        [Fact]
        public void ShouldPropagateParentId()
        {
            // arrange
            const string? testName = nameof(ShouldPropagateParentId);
            var settings = new Dictionary<string, string> {{"VFTelemetry", ""}};
            var consumerConfig = new ConsumerConfig {GroupId = "consumer-group"};
            const string? expectedParentId = "00-3a3fe5cac9c5744b809d4c46b0f9b17d-959f2cb6be067d4c-00";
            var headers = new Headers {{"traceparent", Encoding.ASCII.GetBytes(expectedParentId)}};
            var message = new Message<Ignore, string>
            {
                Value = "Tests",
                Headers = headers
            };
            var consumerMock = new Mock<IConsumer<Ignore, string>>();
            consumerMock.Setup(x => x.Consume(It.IsAny<CancellationToken>()))
                .Returns(new ConsumeResult<Ignore, string>
                {
                    Message = message
                }).Verifiable();

            // act
            using ActivitySource activitySource = new(testName);
            var configurationBuilder = new ConfigurationBuilder().AddInMemoryCollection(settings);
            var configuration = configurationBuilder.Build();
            _services.AddOpenTelemetry(configuration, testName, builder => builder.AddOtelKafkaConsumer(consumerMock.Object, consumerConfig, Array.Empty<string>()));
            var provider = _services.BuildServiceProvider();
            var consumer = provider.GetRequiredService<IConsumer<Ignore, string>>();
            consumer.Consume(new CancellationTokenSource().Token);

            // assert
            consumerMock.Verify(x => x.Consume(It.IsAny<CancellationToken>()), Times.Once);
            ActualTraceIds.Should().NotBeEmpty();
            ActualTraceIds.Should().Contain(expectedParentId);
        }

        [Theory]
        [AutoData]
        public void ShouldAddTags(string testBody)
        {
            // arrange
            const string? testName = nameof(ShouldAddTags);
            const string? customTag = "custom.tag";
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = "localhost",
                ClientId = "clientId.id",
                GroupId = "group.id"
            };
            var tags = new Dictionary<string, string>
            {
                ["body"] = testBody,
                ["custom.tag"] = testName
            };
            var expectedTags = new Dictionary<string, string>
            {
                ["messaging.kafka.consumer_group"] = consumerConfig.GroupId,
                ["messaging.url"] = consumerConfig.BootstrapServers,
                ["messaging.kafka.client_id"] = consumerConfig.ClientId,
                ["messaging.system"] = "kafka",
                ["messaging.destination_kind"] = "topic"
            };
            foreach (var (key, value) in tags)
                expectedTags.Add(key, value);

            var message = new Message<Ignore, string>
            {
                Value = testBody,
                Headers = new Headers
                {
                    {customTag, Encoding.ASCII.GetBytes(tags[customTag])}
                }
            };

            var consumerMock = new Mock<IConsumer<Ignore, string>>();
            consumerMock.Setup(x => x.Consume(It.IsAny<CancellationToken>()))
                .Returns(new ConsumeResult<Ignore, string>
                {
                    Message = message
                });

            // act
            using ActivitySource activitySource = new(testName);
            _services.AddOtelKafkaConsumer(consumerMock.Object, consumerConfig, tags.Select(x => x.Key).ToArray());
            var provider = _services.BuildServiceProvider();
            var consumer = provider.GetRequiredService<IConsumer<Ignore, string>>();
            consumer.Consume(new CancellationTokenSource().Token);

            // assert
            ActualTags.Should().NotBeNull();
            ActualTags.Should().Contain(expectedTags!);
        }

        private void AddActivityListener()
        {
            var activityListener = new ActivityListener
            {
                ShouldListenTo = x => x.Name == nameof(KafkaMessageReceiver),
                SampleUsingParentId = (ref ActivityCreationOptions<string> _) => ActivitySamplingResult.AllData,
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
                ActivityStopped = activity =>
                {
                    ActualTraceIds.Add(activity.ParentId);
                    ActualTags = activity.Tags;
                }
            };
            ActivitySource.AddActivityListener(activityListener);
        }
    }
}