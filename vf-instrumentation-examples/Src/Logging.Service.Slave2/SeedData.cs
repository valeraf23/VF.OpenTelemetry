using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Slave2.Entities;
using Slave2.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slave2
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
                var context = services.GetRequiredService<Slave2DbContext>();
                await context.Database.MigrateAsync();

                var data = GetDummyData("slave2");

                var isNotExist = !await context.Datas.AnyAsync();
                if (isNotExist)
                {
                    await context.Datas.AddRangeAsync(data);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating or initializing the database.");
            }
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