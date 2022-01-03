using Application.Slave2;
using Domain.Extensions;
using Domain.ValueObjects;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Domain.Entities;

namespace Infrastructure.Services
{
    public class Slave2Service : ISlave2Service
    {
        private readonly HttpClient _client;

        public Slave2Service(IHttpClientFactory client) => _client = client.CreateClient(HttpClientNames.Slave2Client);

        public async Task<OrdersList> Get()
        {
            var res = await _client.GetFromJsonAsync<List<Order>>("Slave2/all");
            return res.ToOrderList();
        }

        public async Task<int> SlaveToSlaveFactorial(int number) =>
            await _client.GetFromJsonAsync<int>($"Slave2/slave2-slave2-factorial/{number}");

        public async Task<int> MasterToSlaveFactorial(int number) =>
            await _client.GetFromJsonAsync<int>($"Slave2/master-slave2-factorial/{number}");

        public async Task<DataList> GetData()
        {
            var res = await _client.GetFromJsonAsync<List<User>>("Slave2/data");
            return res.ToDataList();
        }
    }
}