using Application;
using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;

namespace Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            // services.UseInMemoryDatabase();
            // services.UseSqlServer(configuration);
            services.UseMySql(configuration);

            services.AddScoped<IMasterDbContext>(provider => provider.GetService<MasterDbContext>());
            return services;
        }

        private static void UseInMemoryDatabase(this IServiceCollection services) =>
            services.AddDbContext<MasterDbContext>(options =>
                options.UseInMemoryDatabase("Telemetry"));

        private static void UseSqlServer(this IServiceCollection services, IConfiguration configuration) =>
            services.AddDbContext<MasterDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SqlServer"),
                        o =>
                            o.EnableRetryOnFailure(30, TimeSpan.FromSeconds(30), null))
                    .UseLoggerFactory(LoggerFactory.Create(b => b.AddConsole()
                        .AddFilter(l => l == LogLevel.Information)))
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors());

        private static void UseMySql(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionStrings = configuration.GetConnectionStringForMySql();

            services.AddDbContext<MasterDbContext>(options =>
                options.UseMySql(connectionStrings, new MySqlServerVersion(new Version(8, 0, 23)),
                        mySqlOptions => mySqlOptions
                            .EnableRetryOnFailure().SchemaBehavior(MySqlSchemaBehavior.Ignore)
                            .MigrationsHistoryTable("__EFMigrationsHistory", "test"))
                    .ReplaceService<IMigrationsAssembly, DbSchemaAwareMigrationAssembly>()
                    .UseLoggerFactory(LoggerFactory.Create(b => b.AddConsole()
                        .AddFilter(l => l == LogLevel.Information)))
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors());

            services.AddTransient(_ => new MySqlConnection(connectionStrings));
        }
    }
}