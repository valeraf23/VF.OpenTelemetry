using System.Threading.Tasks;

namespace RedisCache
{
    public interface IRedisCacheService
    {
        Task<string> GetValue(string key);
        Task SetValue(string key, string value);
    }
}