using Application.Slave1;
using Domain.Entities;
using Domain.Extensions;
using Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class Slave1Service : ISlave1Service
    {

        private readonly HttpClient _client;

        public Slave1Service(IHttpClientFactory client) => _client = client.CreateClient(HttpClientNames.Slave1Client);

        private async Task<List<Order>> OrdersAsync(string path) => await _client.GetFromJsonAsync<List<Order>>(path);

        public async Task<OrdersList> Get()
        {
            var res = await _client.GetFromJsonAsync<List<Order>>("Slave1");
            return res.ToOrderList();
        }

        public async Task<OrdersList> Parallel(string buyer)
        {
            var baseUrl = $"Slave1/{buyer}";
            var pathStrings = new[]
            {
                $"{baseUrl}/inprogress",
                $"{baseUrl}/notBundle",
                $"{baseUrl}/lost",
                $"{baseUrl}/completed"
            };

            var coverTasksQuery =
                from path
                    in pathStrings
                select OrdersAsync(path);

            var coverTasks = coverTasksQuery.ToList();

            var res = await Task.WhenAll(coverTasks);
            return res.SelectMany(x => x).ToOrderList();
        }

        public async Task<DataList> GetData()
        {
            var res = await _client.GetFromJsonAsync<List<User>>("Slave1/data");
            return res.ToDataList();
        }
    }
}
