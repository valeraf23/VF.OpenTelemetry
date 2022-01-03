using Application.Master;
using Application.Metrics;
using Application.Slave1;
using Application.Slave2;
using Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Services;
using System;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, string key)
        {
            var servicesUrls = configuration.GetSection(key).Get<ServicesUrlsConfigurations>();
            services.AddTransient<IDateTime, SalesDateTime>();
            return services
                .AddHttpClients(servicesUrls)
                .AddServices();
        }

        private static IServiceCollection AddHttpClients(this IServiceCollection services, ServicesUrlsConfigurations servicesUrls)
        {
            services.AddHttpClient(HttpClientNames.MasterClient, client =>
            {
                client.Timeout = TimeSpan.FromMinutes(5);
                client.BaseAddress = new Uri(servicesUrls.MasterUrl);
                client.DefaultRequestHeaders.Clear();
            });
            services.AddHttpClient(HttpClientNames.Slave1Client, client =>
            {
                client.Timeout = TimeSpan.FromMinutes(5);
                client.BaseAddress = new Uri(servicesUrls.Slave1Url);
                client.DefaultRequestHeaders.Clear();
            });
            services.AddHttpClient(HttpClientNames.Slave2Client, client =>
            {
                client.Timeout = TimeSpan.FromMinutes(5);
                client.BaseAddress = new Uri(servicesUrls.Slave2Url);
                client.DefaultRequestHeaders.Clear();
            });
            services.AddHttpClient(HttpClientNames.MetricsClient, client =>
            {
                client.Timeout = TimeSpan.FromMinutes(5);
                client.BaseAddress = new Uri(servicesUrls.PrometheusMetricsUrl);
                client.DefaultRequestHeaders.Clear();
            });
            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddTransient<ISlave1Service, Slave1Service>();
            services.AddTransient<IMasterService, MasterService>();
            services.AddTransient<ISlave2Service, Slave2Service>();
            services.AddTransient<IMetricsService, MetricsService>();
            return services;
        }
    }
}