using System.Text;
using Confluent.Kafka;
using FluentAssertions;
using VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Extensions;
using Xunit;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.UnitTests
{
    public class HeadersExtensionsTests
    {

        [Fact]
        public void ShouldGetValueIfExist()
        {
            const string key = "key";
            const string value = "value";
            var headers = new Headers {{key, Encoding.ASCII.GetBytes(value)}};
            var res = headers.TryGetValue(key, out var actualValue);
            res.Should().BeTrue();
            actualValue.Should().BeEquivalentTo(value);
        }

        [Fact]
        public void ShouldReturnFalseIfNotExist()
        {
            const string key = "key";
            const string value = "";
            var headers = new Headers();
            var res = headers.TryGetValue(key, out var actualValue);
            res.Should().BeFalse();
            actualValue.Should().BeEquivalentTo(value);
        }

        [Fact]
        public void ShouldReturnFalseIfNull()
        {
            const string key = "key";
            const string value = "";
            Headers headers = null;
            var res = HeadersExtensions.TryGetValue(headers, key, out var actualValue);
            res.Should().BeFalse();
            actualValue.Should().BeEquivalentTo(value);
        }
    }
}