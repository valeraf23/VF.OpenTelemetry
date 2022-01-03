using AutoFixture;
using FluentAssertions;
using OpenTelemetry.Exporter;
using VF.Logging.OpenTelemetry.Configuration;
using VF.Logging.OpenTelemetry.Extensions;
using System;
using Xunit;

namespace VF.Logging.OpenTelemetry.UnitTests
{
    public class MapOtlpExporterOptionsTests
    {

        [Fact]
        public void ShouldMapFromOtlpExporterDto()
        {
            Fixture fixture = new();
            var dto = fixture.Create<OtlpExporterDto>();
            var optionActual = new OtlpExporterOptions().MapFrom(dto);

            optionActual.Should().Match<OtlpExporterOptions>(x =>
                x.Endpoint == dto.Endpoint
                && x.Headers == dto.Headers
                && x.ExportProcessorType == dto.ExportProcessorType
                && x.BatchExportProcessorOptions.ExporterTimeoutMilliseconds == dto.BatchExportProcessorOptions.ExporterTimeoutMilliseconds
            );
        }

        [Fact]
        public void ShouldLeftDefaultValuesInCaseOnNull()
        {
            var dto = new OtlpExporterDto
            {
                ExportProcessorType = null,
                BatchExportProcessorOptions = null
            };
            var optionActual = new OtlpExporterOptions().MapFrom(dto);
            var expected = new OtlpExporterOptions();

            optionActual.Should().Match<OtlpExporterOptions>(x =>
                x.ExportProcessorType == expected.ExportProcessorType
                && x.BatchExportProcessorOptions.ExporterTimeoutMilliseconds ==
                expected.BatchExportProcessorOptions.ExporterTimeoutMilliseconds
                && x.BatchExportProcessorOptions.MaxExportBatchSize ==
                expected.BatchExportProcessorOptions.MaxExportBatchSize
            );
        }

        [Fact]
        public void ShouldThrowExceptionIfArgumentDtoNull()
        {
            Action act = () => new OtlpExporterOptions().MapFrom(null);
            act.Should().Throw<ArgumentNullException>();

        }

        [Fact]
        public void ShouldThrowExceptionIfArgumentOptionNull()
        {
            Action act = () => OtelExporterOptionsExtensions.MapFrom(null, new OtlpExporterDto());
            act.Should().Throw<ArgumentNullException>();

        }

        [Fact]
        public void ShouldThrowExceptionIfPropertyWasNotFound()
        {
            Action act = () => OtelExporterOptionsExtensions.MapFromUseReflection(new OtlpExporterOptionsTest(), new OtlpExporterDto());
            act.Should().Throw<Exception>().WithMessage($"Property \"{nameof(OtlpExporterOptionsTest.Test)}\" was not found in {nameof(OtlpExporterDto)}");
        }

        private class OtlpExporterOptionsTest
        {
            public Uri Endpoint { get; set; }

            public string Headers { get; set; }

            public string? Test { get; set; }
        }
    }
}