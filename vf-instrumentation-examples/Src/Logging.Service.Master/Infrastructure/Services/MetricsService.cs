using Application.Metrics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class MetricsService : IMetricsService
    {

        private readonly HttpClient _client;

        public MetricsService(IHttpClientFactory client)
        {
            _client = client.CreateClient(HttpClientNames.MetricsClient);
        }

        public async Task<string> GetMetrics(CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "PrometheusMetrics");
            var response = await _client.SendAsync(request, cancellationToken);
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
    }
}
