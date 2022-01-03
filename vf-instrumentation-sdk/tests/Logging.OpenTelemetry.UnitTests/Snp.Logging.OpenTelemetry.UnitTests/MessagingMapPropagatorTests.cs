using FluentAssertions;
using VF.Logging.OpenTelemetry.MapPropagator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace VF.Logging.OpenTelemetry.UnitTests
{
    public class MessagingMapPropagatorTests
    {
        public MessagingMapPropagatorTests() => AddActivityListner();

        [Fact]
        public void ShouldPropagateSendMessage()
        {
            // arrange
            const string testName = nameof(ShouldPropagateSendMessage);
            var receivedMessage = new Dictionary<string, string>();
            var sentMessage = new Dictionary<string, string>();
            var receivedTag = new Dictionary<string, string>();
            var sentTag = new KeyValuePair<string, string>("messaging.id", "1");
            using ActivitySource activitySource = new(testName);

            // act
            MessagingMapPropagator.SendMessage(
                activitySource,
                sentMessage,
                (props, propKey, propValue) => props.Add(propKey, propValue),
                msg => Send(msg, receivedMessage),
                testName,
                (_, _) => receivedTag.Add(sentTag.Key, sentTag.Value)
            );

            // assert
            receivedMessage.Should().NotBeEmpty();
            sentMessage.Should().NotBeEmpty();
            receivedMessage.Should().Equal(sentMessage);
            receivedTag.Should().NotBeEmpty();
            receivedTag.Should().Contain(sentTag);
        }

        [Fact]
        public async Task ShouldPropagateSendMessageWithResult()
        {
            // arrange
            const string testName = nameof(ShouldPropagateSendMessageWithResult);
            var receivedMessage = new Dictionary<string, string>();
            var sentMessage = new Dictionary<string, string>();
            var receivedTag = new Dictionary<string, string>();
            var sentTag = new KeyValuePair<string, string>("messaging.id", "1");
            using ActivitySource activitySource = new(testName);

            // act
            var res = await MessagingMapPropagator.SendMessage(
                activitySource,
                sentMessage,
                (props, propKey, propValue) => props.Add(propKey, propValue),
                msg =>
                {
                    Send(msg, receivedMessage);
                    return Task.FromResult(true);
                },
                testName,
                (_, _) => receivedTag.Add(sentTag.Key, sentTag.Value)
            );

            // assert
            receivedMessage.Should().NotBeEmpty();
            sentMessage.Should().NotBeEmpty();
            receivedMessage.Should().Equal(sentMessage);
            receivedTag.Should().NotBeEmpty();
            receivedTag.Should().Contain(sentTag);
            res.Should().BeTrue();
        }

        [Fact]
        public void ShouldPropagateReceiveMessage()
        {
            // arrange
            const string testName = nameof(ShouldPropagateReceiveMessage);
            var receivedMessage = new Dictionary<string, string>
            {
                ["traceparent"] = "00-3a3fe5cac9c5744b809d4c46b0f9b17d-959f2cb6be067d4c-00"
            };
            var actuallyTraceParent = string.Empty;
            using ActivitySource activitySource = new(testName);

            // act
            var res = MessagingMapPropagator.ReceiveMessage(
                activitySource,
                Getter,
                () => receivedMessage,
                testName,
                (activity, _) => { actuallyTraceParent = activity?.ParentId; }
            );

            // assert
            receivedMessage.Should().NotBeEmpty();
            actuallyTraceParent.Should().BeEquivalentTo(receivedMessage["traceparent"]);
            res.Should().NotBeEmpty();
            res.Should().BeEquivalentTo(receivedMessage);
        }

        public static IEnumerable<object[]> SplitCountData() => new[]
            {
                new object[] {TimeSpan.FromSeconds(1)},
                new object[] {TimeSpan.FromMilliseconds(100)}
            };

        private static IEnumerable<string> Getter(Dictionary<string, string> props, string key) => props.TryGetValue(key, out var value) ? new[] { value } : Array.Empty<string>();

        private static void AddActivityListner()
        {
            var activityListener = new ActivityListener
            {
                ShouldListenTo = _ => true,
                SampleUsingParentId = (ref ActivityCreationOptions<string> _) => ActivitySamplingResult.AllData,
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData
            };
            ActivitySource.AddActivityListener(activityListener);
        }

        private static void Send(Dictionary<string, string> msg, IDictionary<string, string> receiver)
        {
            foreach (var (key, value) in msg) receiver.Add(key, value);
        }
    }
}