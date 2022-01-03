using Application.Master;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services
{

    public class MasterService : IMasterService
    {
        private readonly HttpClient _client;

        public MasterService(IHttpClientFactory client) => _client = client.CreateClient(HttpClientNames.MasterClient);

        public async Task<int> RecursiveTraces(int counter) =>
            await _client.GetFromJsonAsync<int>($"data/recursive?counter={counter - 1}");

    }
}