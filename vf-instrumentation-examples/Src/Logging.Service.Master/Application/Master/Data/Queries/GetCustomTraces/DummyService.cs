using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using System.Threading.Tasks;

namespace Application.Master.Data.Queries.GetCustomTraces
{
    public class DummyService
    {
        private readonly ILogger<DummyService> _logger;

        public DummyService(ILogger<DummyService> logger) => _logger = logger;

        public async Task DoSomeWork()
        {
            // imitation some work
            await Task.Delay(100);
            var current = Tracer.CurrentSpan.Context;
            _logger.LogInformation($"---------Info From {nameof(DummyService)} TraceId: {current.TraceId} and SpanId: {current.SpanId}--------");
        }

        public async Task DoSomeWorkForPerformanceTraces()
        {
            // imitation some work
            await Task.Delay(100);
        }
    }
}