using Confluent.Kafka;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Consumer.Implementation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Xunit;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.UnitTests
{
    public class SdkKafkaConsumerTests
    {
        public SdkKafkaConsumerTests() => AddActivityListener();

        private List<string?> ActualTraceIds { get; } = new();

        [Fact]
        public void ShouldPropagateParentId()
        {
            // arrange
            const string? testName = nameof(ShouldPropagateParentId);
            var consumerConfig = new ConsumerConfig { GroupId = "consumer-group" };
            const string? expectedParentId = "00-3a3fe5cac9c5744b809d4c46b0f9b17d-959f2cb6be067d4c-00";
            var headers = new Headers { { "traceparent", Encoding.ASCII.GetBytes(expectedParentId) } };
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

            using ActivitySource activitySource = new(testName);
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

            // act
            var consumer = LoggingKafkaSdk.CreateConsumer(consumerConfig, consumerMock.Object, loggerFactory, Array.Empty<string>());
            consumer.Consume(new CancellationTokenSource().Token);

            // assert
            consumerMock.Verify(x => x.Consume(It.IsAny<CancellationToken>()), Times.Once);
            ActualTraceIds.Should().NotBeEmpty();
            ActualTraceIds.Should().Contain(expectedParentId);
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
                }
            };
            ActivitySource.AddActivityListener(activityListener);
        }
    }
}