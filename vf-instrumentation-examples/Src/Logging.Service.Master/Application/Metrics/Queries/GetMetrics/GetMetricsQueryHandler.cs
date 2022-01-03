using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Metrics.Queries.GetMetrics
{
    public class GetMetricsQueryHandler : IRequestHandler<GetMetrics, string>
    {
        private readonly IMetricsService _service;

        public GetMetricsQueryHandler(IMetricsService service) => _service = service;

        public async Task<string> Handle(GetMetrics request, CancellationToken cancellationToken) =>
            await _service.GetMetrics(cancellationToken);
    }
}
