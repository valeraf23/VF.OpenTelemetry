using Microsoft.Extensions.Configuration;

namespace Slave1.Helpers
{
    public static class ConnectionStringHelper
    {

        public static string GetSlave1(this IConfiguration configuration)
        {
            var connectionString = GetConnectionString(configuration);
            return string.Format(connectionString, "Slave1");
        }
        public static string GetMaster(this IConfiguration configuration)
        {
            var connectionString = GetConnectionString(configuration);
            return string.Format(connectionString, "master");
        }

        private static string GetConnectionString(IConfiguration configuration) => configuration.GetSection("SqlServer").Value;
    }
}