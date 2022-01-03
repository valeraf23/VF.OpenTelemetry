using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using VF.Logging.OpenTelemetry.VfTracer;
using Xunit;

namespace VF.Logging.OpenTelemetry.UnitTests
{
    public class TracerTests
    {
        private readonly ServiceCollection _services;

        public TracerTests()
        {
            _services = new ServiceCollection();
            _services.AddLogging();
            AddActivityListener();
        }

        private string? DisplayName { get; set; }

        [Fact]
        public void AddDefaultVfTracerMessage()
        {
            // arrange
            const string? testName = nameof(AddDefaultVfTracerMessage);
            var settings = new Dictionary<string, string> { { "VFTelemetry", "" } };

            // act
            using ActivitySource activitySource = new(testName);
            var configurationBuilder = new ConfigurationBuilder().AddInMemoryCollection(settings);
            var configuration = configurationBuilder.Build();
            _services.AddOpenTelemetry(configuration, testName);
            var provider = _services.BuildServiceProvider();
            var vfTracer = provider.GetRequiredService<ITracer>();
            vfTracer.AddSpan(() => Thread.Sleep(100));

            // assert
            DisplayName.Should().NotBeNull();
            DisplayName.Should().BeEquivalentTo("dotnet.custom.method.span");
        }

        [Fact]
        public async Task AddDefaultVfTracerMessageForFunc()
        {
            // arrange
            const string? testName = nameof(AddDefaultVfTracerMessage);
            var settings = new Dictionary<string, string> { { "VFTelemetry", "" } };

            // act
            using ActivitySource activitySource = new(testName);
            var configurationBuilder = new ConfigurationBuilder().AddInMemoryCollection(settings);
            var configuration = configurationBuilder.Build();
            _services.AddOpenTelemetry(configuration, testName);
            var provider = _services.BuildServiceProvider();
            var vfTracer = provider.GetRequiredService<ITracer>();
            await vfTracer.AddSpan(async () => await Task.Delay(100));

            // assert
            DisplayName.Should().NotBeNull();
            DisplayName.Should().BeEquivalentTo("dotnet.custom.method.span");
        }

        [Fact]
        public void AddSdkDefaultVfTracerMessage()
        {
            // arrange
            const string? testName = nameof(AddDefaultVfTracerMessage);

            // act
            using ActivitySource activitySource = new(testName);
            Sdk.AddSpan(() => Thread.Sleep(100));

            // assert
            DisplayName.Should().NotBeNull();
            DisplayName.Should().BeEquivalentTo("dotnet.custom.method.span");
        }

        private void AddActivityListener()
        {
            var activityListener = new ActivityListener
            {
                ShouldListenTo = _ => true,
                SampleUsingParentId = (ref ActivityCreationOptions<string> _) => ActivitySamplingResult.AllData,
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
                ActivityStopped = activity => DisplayName = activity.DisplayName
            };
            ActivitySource.AddActivityListener(activityListener);
        }
    }
}