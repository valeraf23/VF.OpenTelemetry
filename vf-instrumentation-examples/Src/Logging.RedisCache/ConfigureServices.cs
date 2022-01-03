using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace RedisCache
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddRedisCacheService(this IServiceCollection services, string redisConnection)
        {
            services.AddSingleton<IRedisCacheService, RedisCacheService>();
            services.AddSingleton<IConnectionMultiplexer>(x => ConnectionMultiplexer.Connect(redisConnection));
            return services;
        }
    }
}