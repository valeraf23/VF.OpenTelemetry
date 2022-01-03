using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RedisCache;
using Slave1.Entities;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Slave1.Helpers;

namespace Slave1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Slave1Controller : ControllerBase
    {
        private const string Key = "data";
        private readonly IConfiguration _configuration;
        private readonly IRedisCacheService _redisCacheService;

        public Slave1Controller(IRedisCacheService redisCacheService, IConfiguration configuration)
        {
            _redisCacheService = redisCacheService;
            _configuration = configuration;
        }

        private async Task<string> GetAllDataAndCached()
        {
            var data = await GetAllData();
            var jsonData = JsonSerializer.Serialize(data);
            await _redisCacheService.SetValue(Key, jsonData);
            return jsonData;
        }

        [HttpGet]
        [Route("data")]
        public async Task<string> GetData()
        {
            var cachedData = await _redisCacheService.GetValue(Key);
            if (!string.IsNullOrEmpty(cachedData)) return cachedData;
            return await GetAllDataAndCached();
        }

        private async Task<List<Data>> GetAllData()
        {
            var dataList = new List<Data>();

            await using var connection = new SqlConnection(_configuration.GetSlave1());
            await connection.OpenAsync();

            const string sql =
                "select Data.Id, Data.Value , SubData.Value from Data join SubData on Data.Id = SubData.Data";

            await using var command = new SqlCommand(sql, connection);
            await using var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            while (await reader.ReadAsync())
            {
                var id = reader.GetInt32(0);
                var value = reader.GetString(1);
                var subData = new SubData
                {
                    Value = reader.GetString(2)
                };

                var data = dataList.FirstOrDefault(d => d.Id == id);
                if (data is null)
                {
                    var newData = new Data
                    {
                        Id = id,
                        Value = value,
                        SubData = new List<SubData> { subData }
                    };
                    dataList.Add(newData);
                }
                else
                {
                    data.SubData.Add(subData);
                }
            }

            return dataList;
        }
    }
}