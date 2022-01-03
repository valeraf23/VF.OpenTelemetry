using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OpenTelemetry;
using OpenTelemetry.Trace;
using VF.Logging.OpenTelemetry.Configuration;
using VF.Logging.OpenTelemetry.Extensions.TracerProviderBuilderExtensions;
using VF.Logging.OpenTelemetry.TraceInstrumentation;
using VF.Logging.OpenTelemetry.TraceInstrumentation.Implementation;
using StackExchange.Redis;
using Xunit;

namespace VF.Logging.OpenTelemetry.UnitTests
{
    public class AddInstrumentationTests
    {
        private readonly ServiceCollection _services;
        private readonly OpenTelemetrySetup _openTelemetrySetup;

        public AddInstrumentationTests()
        {
            _services = new ServiceCollection();
            _openTelemetrySetup = new OpenTelemetrySetup(_services);
        }

        //https://github.com/open-telemetry/opentelemetry-dotnet/issues/2091
        [Fact]
        public void CollectionWasModifiedEnumerationOperationMayNotExecute()
        {
            // arrange
            _services.AddOpenTelemetryTracing(builder =>
            {
                builder.Configure(
                    (_, b) => b.AddAspNetCoreInstrumentation());
            });
            var provider = _services.BuildServiceProvider();
            var tracerFactory = provider.GetRequiredService<TracerProvider>();
        }

        [Fact]
        public void ShouldAddRedisInstrumentation()
        {
            // arrange
            _services.AddOptions();
            var mock = new Mock<IConnectionMultiplexer>();
            _services.AddSingleton(mock.Object);

            // act
            _openTelemetrySetup.AddInstrumentation(new List<string> {"Redis"});
            var provider = _services.BuildServiceProvider();
            var service = provider.GetService<IInstrumentation>();

            // assert
            service.Should().NotBeNull();
            service.Should().BeAssignableTo<RedisInstrumentation>();
        }

        [Fact]
        public void ShouldThrowExceptionInCaseOfInCorrectParseTimeSpanArgumentsRedisInstrumentation()
        {
            // arrange
            var flushInterval = new FlushInterval
            {
                Format = "test",
                Interval = 5
            };
            _services.Configure<RedisConfiguration>(x => x.FlushInterval = flushInterval);
            var mock = new Mock<IConnectionMultiplexer>();
            _services.AddSingleton(mock.Object);

            // act
            _openTelemetrySetup.AddInstrumentation(new List<string> {"Redis"});
            var provider = _services.BuildServiceProvider();
            Action act = () => global::OpenTelemetry.Sdk.CreateTracerProviderBuilder().AddTraceInstrumentation(provider);

            // assert
            act.Should().Throw<ArgumentException>().WithMessage(
                $"Could not parse to timeSpan, Incorrect arguments : {JsonSerializer.Serialize(flushInterval)}");
        }

        [Fact]
        public void ShouldCallAddMethod()
        {
            // arrange
            var mocks = Enumerable.Range(0, 5).Select(_ => new Mock<IInstrumentation>()).ToArray();
            foreach (var moq in mocks)
            {
                moq.Setup(m => m.Add(It.IsAny<TracerProviderBuilder>())).Verifiable();
                _services.AddSingleton(moq.Object);
            }

            // act
            var provider = _services.BuildServiceProvider();
            global::OpenTelemetry.Sdk.CreateTracerProviderBuilder().AddTraceInstrumentation(provider);

            // assert
            foreach (var moq in mocks) moq.Verify(x => x.Add(It.IsAny<TracerProviderBuilder>()), Times.Exactly(1));
        }

        [Fact]
        public void ShouldAddAspNetCoreInstrumentation()
        {
            // arrange
            _services.AddOptions();

            // act
            _openTelemetrySetup.AddInstrumentation(new List<string> {"AspNetCore"});

            var provider = _services.BuildServiceProvider();
            var service = provider.GetService<IInstrumentation>();

            // assert
            service.Should().NotBeNull();
            service.Should().BeAssignableTo<AspNetCoreInstrumentation>();
        }

        [Fact]
        public void ShouldAddSqlClientInstrumentation()
        {
            // arrange
            _services.AddOptions();

            // act
            _openTelemetrySetup.AddInstrumentation(new List<string> {"Sql"});

            var provider = _services.BuildServiceProvider();
            var service = provider.GetService<IInstrumentation>();

            // assert
            service.Should().NotBeNull();
            service.Should().BeAssignableTo<SqlClientInstrumentation>();
        }

        [Fact]
        public void ShouldAddHttpClientInstrumentation()
        {
            // act
            _openTelemetrySetup.AddInstrumentation(new List<string> {"HttpClient"});

            var provider = _services.BuildServiceProvider();
            var service = provider.GetService<IInstrumentation>();

            // assert
            service.Should().NotBeNull();
            service.Should().BeAssignableTo<HttpClientInstrumentation>();
        }

        [Fact]
        public void ShouldAddInstrumentation()
        {
            // act
            _openTelemetrySetup.SetInstrumentation(Assembly.GetAssembly(typeof(TestInstrumentation)));
            _openTelemetrySetup.AddInstrumentation(new[] {nameof(TestInstrumentation)});
            var provider = _services.BuildServiceProvider();
            global::OpenTelemetry.Sdk.CreateTracerProviderBuilder().AddTraceInstrumentation(provider);

            // assert
            var service = provider.GetService<IInstrumentation>();
            service.Should().NotBeNull();
            service.Should().BeAssignableTo<TestInstrumentation>();
        }

        [Fact]
        public void ShouldThrowExceptionIfAssemblyWasNotFound()
        {
            // act
            Action act = () => _openTelemetrySetup.SetInstrumentation(null);

            // assert
            act.Should().Throw<ArgumentNullException>();
        }

        private class TestInstrumentation : IInstrumentation
        {
            public TracerProviderBuilder Add(TracerProviderBuilder builder)
            {
                return builder;
            }
        }
    }
}