using System;
using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;
using VF.Logging.OpenTelemetry.Configuration;
using StackExchange.Redis;

namespace VF.Logging.OpenTelemetry.TraceInstrumentation.Implementation
{
    public class RedisInstrumentation : IInstrumentation
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly RedisConfiguration _redisConfigurationOptions;

        public RedisInstrumentation(IOptions<RedisConfiguration> redisOptions,
            IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _redisConfigurationOptions = redisOptions.Value;
        }

        public TracerProviderBuilder Add(TracerProviderBuilder builder)
        {
            var isExact = TimeSpan.TryParseExact(_redisConfigurationOptions.FlushInterval.Interval.ToString(),
                _redisConfigurationOptions.FlushInterval.Format,
                CultureInfo.CurrentCulture, out var flushInterval);

            if (!isExact)
                throw new ArgumentException(
                    $"Could not parse to timeSpan, Incorrect arguments : {JsonSerializer.Serialize(_redisConfigurationOptions.FlushInterval)}");

            return builder
                .AddRedisInstrumentation(_connectionMultiplexer,
                    options => options.FlushInterval = flushInterval);
        }
    }
}