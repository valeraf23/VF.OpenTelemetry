namespace VF.Logging.OpenTelemetry.Configuration
{
    public class BatchExportProcessorOptionsDto
    {
        public int? MaxQueueSize { get; set; }
        public int? ScheduledDelayMilliseconds { get; set; }
        public int? ExporterTimeoutMilliseconds { get; set; }
        public int? MaxExportBatchSize { get; set; }
    }
}