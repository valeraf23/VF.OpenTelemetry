using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using VF.Logging.OpenTelemetry;
using VF.Logging.OpenTelemetry.VfTracer;

namespace Application.Master.Data.Queries.GetCustomTraces
{
    public class GetCustomWithSpan : IRequest
    {

        internal class GetCustomWithSpanHandler : IRequestHandler<GetCustomWithSpan>
        {
            private readonly IVfTracer _tracer;
            private readonly DummyService _dummyService;
            private readonly ILogger<GetCustomWithSpan> _logger;

            public GetCustomWithSpanHandler(ILogger<GetCustomWithSpan> logger, IVfTracer tracer, DummyService dummyService)
            {
                _logger = logger;
                _tracer = tracer;
                _dummyService = dummyService;
            }

            public Task<Unit> Handle(GetCustomWithSpan request, CancellationToken cancellationToken)
            {
                _tracer.AddSpan(async ()=> await _dummyService.DoSomeWorkForPerformanceTraces());
                _logger.LogInformation($"---{DateTime.Now.ToShortTimeString()} Custom traces With-Span was created --- ");
                return Task.FromResult(Unit.Value);
            }
        }
    }
}