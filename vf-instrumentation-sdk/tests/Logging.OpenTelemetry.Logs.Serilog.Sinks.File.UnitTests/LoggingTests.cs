using System;
using System.Collections.Generic;
using System.Diagnostics;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog.Core;
using Serilog.Events;
using Xunit;

namespace VF.Logging.OpenTelemetry.Logs.Serilog.Sinks.File.UnitTests
{
    public class LoggingTests
    {
        [Fact]
        public void ShouldWriteToFile()
        {
            // arrange
            const string text = "Hello";
            using var tmp = TempFolder.ForCaller();

            // act
            var path = tmp.AllocateFilename("log");

            using (var loggerFactory = LoggerFactory.Create(builder => builder.AddVfLog(x => x.Path = path, true)))
            {
                loggerFactory.CreateLogger<LoggingTests>().LogInformation(text);
            }

            var lines = System.IO.File.ReadAllLines(path);

            // assert
            lines.Should().Contain(text);
        }

        [Fact]
        public void ShouldWriteToFileFromConfiguration()
        {
            // arrange

            const string text = "Hello";
            using var tmp = TempFolder.ForCaller();

            // act
            var path = tmp.AllocateFilename("log");
            var settings = new Dictionary<string, string>
            {
                {$"VFLogging:{nameof(LoggingConfiguration.Path)}", path}
            };

            var configurationBuilder = new ConfigurationBuilder().AddInMemoryCollection(settings);
            var configuration = configurationBuilder.Build();

            using (var loggerFactory = LoggerFactory.Create(builder => builder.AddVfLog(configuration, true)))
            {
                loggerFactory.CreateLogger<LoggingTests>().LogInformation(text);
            }

            var lines = System.IO.File.ReadAllLines(path);

            // assert
            lines.Should().Contain(text);
        }

        [Fact]
        public void ShouldAddTraceIdAndSpanId()
        {
            // arrange
            const string testName = nameof(ActivityEnricherAddActivitesProperties);
            var activityListener = new ActivityListener
            {
                ShouldListenTo = _ => true,
                SampleUsingParentId = (ref ActivityCreationOptions<string> _) => ActivitySamplingResult.AllData,
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData
            };
            ActivitySource.AddActivityListener(activityListener);

            using ActivitySource activitySource = new(testName);
            using var activity = activitySource.StartActivity(testName);
            if(activity is null) Assert.False(true, "activity should not be null");
            const string text = "Hello";
            using var tmp = TempFolder.ForCaller();

            // act
            var path = tmp.AllocateFilename("log");
            using (var loggerFactory = LoggerFactory.Create(builder => builder.AddVfLog(x => x.Path = path, true)))
            {
                loggerFactory.CreateLogger<LoggingTests>().LogInformation(text);
            }

            var lines = System.IO.File.ReadAllLines(path);

            // assert
            lines.Should().Contain(text)
                .And
                .Contain(x => x.Contains(activity.GetSpanId()))
                .And
                .Contain(x => x.Contains(activity.GetTraceId()));
        }

        [Fact]
        public void ActivityEnricherAddActivitesProperties()
        {
            // arrange
            const string testName = nameof(ActivityEnricherAddActivitesProperties);
            var activityListener = new ActivityListener
            {
                ShouldListenTo = _ => true,
                SampleUsingParentId = (ref ActivityCreationOptions<string> _) => ActivitySamplingResult.AllData,
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData
            };
            ActivitySource.AddActivityListener(activityListener);

            using ActivitySource activitySource = new(testName);
            using var activity = activitySource.StartActivity(testName);

            var logEvent = new LogEvent(DateTimeOffset.Now, LogEventLevel.Information, new Exception(),
                MessageTemplate.Empty, new List<LogEventProperty>());
            var logEventPropertyFactoryMock = new Mock<ILogEventPropertyFactory>();

            // act
            new ActivityEnricher().Enrich(logEvent, logEventPropertyFactoryMock.Object);

            // assert
            logEvent.Properties.Should().HaveCount(3);
        }
    }
}