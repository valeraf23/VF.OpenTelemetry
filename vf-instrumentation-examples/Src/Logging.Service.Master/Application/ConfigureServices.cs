using Application.Common;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Application.Master.Data.Queries.GetCustomTraces;

namespace Application
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplication(this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {

            serviceCollection.Configure<ConnectionStrings>(configuration.GetSection("ConnectionStrings"));
            serviceCollection.Configure<ConnectionStrings>(x =>
            {
                x.SqlServer = configuration.GetSection("ConnectionStrings:SqlServer").Value;
                x.MySql = configuration.GetConnectionStringForMySql();
            });
            serviceCollection.AddMediatR(Assembly.GetExecutingAssembly());
            serviceCollection.AddTransient<DummyService>();
            return serviceCollection;
        }
    }

    public static class MySqlConnectionString
    {
        public static string GetConnectionStringForMySql(this IConfiguration configuration)
        {
            var host = configuration["DBHOST"] ?? configuration.GetConnectionString("MYSQL_HOST");
            var port = configuration["DBPORT"] ?? configuration.GetConnectionString("MYSQL_PORT");
            var password = configuration["MYSQL_PASSWORD"] ?? configuration.GetConnectionString("MYSQL_PASSWORD");
            var userid = configuration["MYSQL_USER"] ?? configuration.GetConnectionString("MYSQL_USER");
            var usersDataBase = configuration["MYSQL_DATABASE"] ?? configuration.GetConnectionString("MYSQL_DATABASE");
            var connString = $"server={host}; port={port}; userid={userid};pwd={password};database={usersDataBase}";
            return connString;
        }
    }
}