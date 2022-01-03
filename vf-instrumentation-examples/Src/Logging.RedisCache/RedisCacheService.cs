using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisCache
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDatabase _db;

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            _db = connectionMultiplexer.GetDatabase();
        }

        public async Task<string> GetValue(string key)
        {
            return await _db.StringGetAsync(key);
        }

        public async Task SetValue(string key, string value)
        {
            await _db.StringSetAsync(key, value, TimeSpan.FromSeconds(15));
        }
    }
}
