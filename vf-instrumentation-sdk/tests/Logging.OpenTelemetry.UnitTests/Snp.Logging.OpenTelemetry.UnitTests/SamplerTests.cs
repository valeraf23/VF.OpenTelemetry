using FluentAssertions;
using OpenTelemetry.Trace;
using VF.Logging.OpenTelemetry.Extensions.TracerProviderBuilderExtensions;
using System;
using System.Reflection;
using Xunit;

namespace VF.Logging.OpenTelemetry.UnitTests
{
    public class SamplerTests
    {
        [Theory]
        [InlineData("off")]
        [InlineData("OFF")]
        public void ShouldGetAlwaysOffSampler(string key)
        {
            var sampler = SamplerTracerProviderBuilderExtensions.AddSampler(key);
            sampler.Should().BeOfType<AlwaysOffSampler>();
        }

        [Theory]
        [InlineData("on")]
        [InlineData("On")]
        [InlineData("")]
        public void ShouldGetAlwaysOnSampler(string key)
        {
            var sampler = SamplerTracerProviderBuilderExtensions.AddSampler(key);
            sampler.Should().BeOfType<AlwaysOnSampler>();
        }

        [Theory]
        [InlineData("0.5")]
        [InlineData("0.1")]
        [InlineData("0.99")]
        public void ShouldGetTraceIdRatioBasedSampler(string key)
        {
            var sampler = SamplerTracerProviderBuilderExtensions.AddSampler(key);
            sampler.Should().BeOfType<TraceIdRatioBasedSampler>();

            var ratio = typeof(TraceIdRatioBasedSampler)
                .GetField("probability", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(sampler);
            ratio.ToString().Should().Be(key);
        }

        [Fact]
        public void ShouldGetTraceIdRatioBasedSamplerWithComma()
        {
            const string? key = "0,2";
            var sampler = SamplerTracerProviderBuilderExtensions.AddSampler(key);
            sampler.Should().BeOfType<TraceIdRatioBasedSampler>();

            var ratio = typeof(TraceIdRatioBasedSampler)
                .GetField("probability", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(sampler);
            ratio.ToString().Should().Be("0.2");
        }

        [Fact]
        public void ShouldThrowExceptionWhenArgumentHasInvalidValue()
        {
            const string? key = "test";
            Action addSampler = () => SamplerTracerProviderBuilderExtensions.AddSampler(key);
            addSampler.Should().Throw<Exception>()
                .WithMessage($"Invalid key:{key}. Use \"On\",\"Off\" or double value in range from 0 to 1");
        }

        [Theory]
        [InlineData("-0.5")]
        [InlineData("1.2")]
        public void ShouldThrowExceptionWhenRatioInRangeLessZeroOrGreatOne(string key)
        {
            Action addSampler = () => SamplerTracerProviderBuilderExtensions.AddSampler(key);
            addSampler.Should().Throw<Exception>().WithMessage("Ratio have to be in range from 0 to 1");
        }
    }
}