using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using VF.Logging.OpenTelemetry.Configuration;
using Xunit;

namespace VF.Logging.OpenTelemetry.UnitTests
{
    public class OpenTelemetrySetupTests
    {
        private readonly ServiceCollection _services;
        private readonly OpenTelemetrySetup _openTelemetrySetup;

        public OpenTelemetrySetupTests()
        {
            _services = new ServiceCollection();
            _openTelemetrySetup = new OpenTelemetrySetup(_services);
        }

        [Fact]
        public void ShouldAddExporterOption()
        {
            // arrange
            var otlpExporterDto = new OtlpExporterDto();

            // act
            _openTelemetrySetup.AddExporterOption(otlpExporterDto);
            var provider = _services.BuildServiceProvider();
            var restService = provider.GetService<OtlpExporterDto>();

            // assert
            restService.Should().NotBeNull();
        }

        [Fact]
        public void ShouldSetOtlpExporterOptions()
        {
            // arrange
            const int expectedTimeoutMilliseconds = 10;
            var otlpExporterDto = new OtlpExporterDto();

            // act
            _openTelemetrySetup.SetOtlpExporterOptions(x => x.TimeoutMilliseconds = expectedTimeoutMilliseconds);
            _openTelemetrySetup.AddExporterOption(otlpExporterDto);
            var provider = _services.BuildServiceProvider();
            var restService = provider.GetService<OtlpExporterDto>();

            // assert
            restService.Should().NotBeNull();
            restService!.TimeoutMilliseconds.Should().Be(expectedTimeoutMilliseconds);
        }
    }
}