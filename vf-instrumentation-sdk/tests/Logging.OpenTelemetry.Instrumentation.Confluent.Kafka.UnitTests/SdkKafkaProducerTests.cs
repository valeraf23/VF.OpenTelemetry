using Confluent.Kafka;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Producer.Implementation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SmartWait.Core;
using SmartWait.Results.Extension;
using Xunit;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.UnitTests
{
    public class SdkKafkaProducerTests
    {
        public SdkKafkaProducerTests() => AddActivityListener();

        private List<string?> ActualTraceIds { get; } = new();

        [Fact]
        public async Task ShouldPropagateParentId()
        {
            // arrange
            var producerConfig = new ProducerConfig();
            var message = new Message<Null, string> {Value = "Tests"};
            var producerMock = new Mock<IProducer<Null, string>>();
            producerMock.Setup(x => x.ProduceAsync(It.IsAny<string>(), message, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new DeliveryResult<Null, string>())).Verifiable();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

            // act
            var consumer = LoggingKafkaSdk.CreateProducer(producerConfig, loggerFactory, producerMock.Object,
                Array.Empty<string>());
            await consumer.ProduceAsync(It.IsAny<string>(), message, It.IsAny<CancellationToken>());

            // assert
            producerMock.Verify(mock => mock.ProduceAsync(It.IsAny<string>(), message, It.IsAny<CancellationToken>()),
                Times.Once());

            Func<IHeader, bool> predicate = header => header.Key.Equals("traceparent") &&
                                                        ActualTraceIds.Contains(
                                                            Encoding.ASCII.GetString(header.GetValueBytes()));

            var res = WaitFor.For(() => message.Headers)
                .Become(x => x.Any(h => predicate(h)))
                .WhenNotExpectedValue(x => x.ActuallyValue)
                .OnFailureThrowException();

            res.Should().Contain(x => predicate(x));
        }

        private void AddActivityListener()
        {
            var activityListener = new ActivityListener
            {
                ShouldListenTo = x => x.Name == nameof(KafkaMessageSender),
                SampleUsingParentId = (ref ActivityCreationOptions<string> _) => ActivitySamplingResult.AllData,
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
                ActivityStopped = activity => ActualTraceIds.Add(activity.Id)
            };
            ActivitySource.AddActivityListener(activityListener);
        }
    }
}