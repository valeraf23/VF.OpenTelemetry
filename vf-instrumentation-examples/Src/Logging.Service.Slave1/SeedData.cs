using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Slave1.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Slave1.Helpers;
using SqlCommand = System.Data.SqlClient.SqlCommand;

namespace Slave1
{
    public static class SeedData
    {
        public static async Task Seed(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            try
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetSlave1();
                await InitDb(configuration.GetMaster());
                await AddData(connectionString);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating or initializing the database.");
            }
        }

        private static async Task InitDb(string connectionString)
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), "SqlScripts", "Init.sql");
            var script = await File.ReadAllTextAsync(file);
            
            await using var connection = new SqlConnection(connectionString);
            var server = new Server(new ServerConnection(connection));

            server.ConnectionContext.ExecuteNonQuery(script);
        }

        private static async Task AddData(string connectionString)
        {
            const string sqlExpression = "dbo.SeedData";
            var data = GenerateData("slave1");

            await using var con = new System.Data.SqlClient.SqlConnection(connectionString);
            await using var cmd = new SqlCommand(sqlExpression)
            {
                CommandType = CommandType.StoredProcedure,
                Connection = con
            };
            cmd.Parameters.AddWithValue("@tblData", data);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        private static DataTable GenerateData(string key)
        {
            var dt = new DataTable();
            dt.Columns.AddRange(new[]
            {
                new DataColumn("Data", typeof(string)),
                new DataColumn("SubData", typeof(string))
            });

            var data = GetDummyData(key);

            foreach (var d in data)
                foreach (var sd in d.SubData)
                    dt.Rows.Add(d.Value, sd.Value);

            return dt;
        }

        private static IEnumerable<Data> GetDummyData(string key)
        {
            var r = new Random();

            var data = Enumerable.Range(1, 20).Select(_ =>
            {
                var i = r.Next(10);
                return new Data
                {
                    Value = $"{key}_{r.Next(10)}",
                    SubData = GetDummySubData(key, i)
                };
            });
            return data.ToList();
        }

        private static ICollection<SubData> GetDummySubData(string key, int i)
        {
            var subData = Enumerable.Range(1, 20).Select(_ =>
                new SubData
                {
                    Value = $"{key}_{i}"
                }
            );
            return subData.ToList();
        }
    }
}