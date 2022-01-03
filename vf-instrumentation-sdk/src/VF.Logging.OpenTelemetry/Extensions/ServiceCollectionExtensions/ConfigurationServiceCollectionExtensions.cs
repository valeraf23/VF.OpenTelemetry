using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VF.Logging.OpenTelemetry.Configuration;
using VF.Logging.OpenTelemetry.Extensions.ConfigurationExtensions;

namespace VF.Logging.OpenTelemetry.Extensions.ServiceCollectionExtensions
{
    internal static class ConfigurationServiceCollectionExtensions
    {
        private static IServiceCollection AddTraceInstrumentationConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetTraceInstrumentationSection();
            if (section.Exists())
            {
                return services.Configure<AspNetCoreConfiguration>(section.GetSection("AspNetCore"))
                    .Configure<RedisConfiguration>(section.GetSection("Redis"))
                    .Configure<SqlClientConfiguration>(section.GetSection("SqlClient"))
                    .Configure<EFConfiguration>(section.GetSection("EF"));
            }

            return services;
        }

        private static IServiceCollection AddOpenTelemetryConfiguration(this IServiceCollection services, IConfiguration configuration) =>
            services.AddSingleton(configuration.GetOpenTelemetryConfiguration());

        internal static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
            => services.AddOpenTelemetryConfiguration(configuration)
                .AddTraceInstrumentationConfiguration(configuration);
    }
}