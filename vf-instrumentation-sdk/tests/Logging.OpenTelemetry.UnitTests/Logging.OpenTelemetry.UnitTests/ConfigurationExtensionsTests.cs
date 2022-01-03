using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VF.Logging.OpenTelemetry.Configuration;
using VF.Logging.OpenTelemetry.Extensions.ConfigurationExtensions;
using VF.Logging.OpenTelemetry.Extensions.ServiceCollectionExtensions;
using Xunit;

namespace VF.Logging.OpenTelemetry.UnitTests
{
    public class ConfigurationExtensionsTests
    {
        [Fact]
        public void ShouldReturnGetOpenTelemetryConfiguration()
        {
            // arrange
            var settings = new Dictionary<string, string>
            {
                {"VFTelemetry", ""}
            };

            // act
            var configurationBuilder = new ConfigurationBuilder().AddInMemoryCollection(settings);
            var configuration = configurationBuilder.Build();
            var res = configuration.GetOpenTelemetryConfiguration();

            // assert

            res.Should().NotBeNull();
        }

        [Fact]
        public void ShouldThrowExceptionIfVfTelemetrySectionDoesNotExist()
        {
            // arrange
            var configurationBuilder = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>());
            var configuration = configurationBuilder.Build();

            //act
            Action act = () => configuration.GetOpenTelemetryConfiguration();

            // assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Add VFTelemetry: section in appsettings.json (Parameter 'VFTelemetry')");
        }

        [Fact]
        public void ShouldRegisterOpenTelemetryConfiguration()
        {
            // arrange
            var settings = new Dictionary<string, string>
            {
                {"VFTelemetry", ""}
            };
            var services = new ServiceCollection();

            var configurationBuilder = new ConfigurationBuilder().AddInMemoryCollection(settings);
            var configuration = configurationBuilder.Build();

            //act
            services.AddConfiguration(configuration);
            var provider = services.BuildServiceProvider();
            var service = provider.GetService<OpenTelemetryConfiguration>();

            // assert
            service.Should().NotBeNull();
        }

        [Fact]
        public void ShouldRegisterInstrumentationConfigurations()
        {
            // arrange
            var settings = new Dictionary<string, string>
            {
                {"VFTelemetry:TraceInstrumentation:AspNetCore:RecordException", "true"}
            };
            var services = new ServiceCollection();
            services.AddOptions();
            var configurationBuilder = new ConfigurationBuilder().AddInMemoryCollection(settings);
            var configuration = configurationBuilder.Build();

            //act
            services.AddConfiguration(configuration);
            var provider = services.BuildServiceProvider();
            var service = provider.GetService<IOptions<AspNetCoreConfiguration>>();
            var aspNetCoreConfiguration = service?.Value;

            // assert
            aspNetCoreConfiguration.Should().NotBeNull();
            aspNetCoreConfiguration!.RecordException.Should().BeTrue();
        }
    }
}