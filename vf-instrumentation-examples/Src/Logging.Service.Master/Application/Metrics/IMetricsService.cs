using System.Threading;
using System.Threading.Tasks;

namespace Application.Metrics
{
    public interface IMetricsService
    {
        Task<string> GetMetrics(CancellationToken cancellationToken);
    }
}