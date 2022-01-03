using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Master.Data.Queries.GetCustomTraces
{
    public class GetCustomWithOutSpan : IRequest
    {
        internal class GetCustomWithOutSpanHandler : IRequestHandler<GetCustomWithOutSpan>
        {
            private readonly DummyService _dummyService;
            private readonly ILogger<GetCustomWithOutSpan> _logger;

            public GetCustomWithOutSpanHandler(ILogger<GetCustomWithOutSpan> logger, DummyService dummyService)
            {
                _logger = logger;
                _dummyService = dummyService;
            }

            public async Task<Unit> Handle(GetCustomWithOutSpan request, CancellationToken cancellationToken)
            {
                await _dummyService.DoSomeWorkForPerformanceTraces();
                _logger.LogInformation($"---{DateTime.Now.ToShortTimeString()} Custom traces WithOut-Span was created --- ");
                return Unit.Value;
            }
        }
    }
}