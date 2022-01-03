using System;
using System.Threading;
using System.Threading.Tasks;
using Application.System.Commands.SeedSampleData;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Master.Helpers
{
    public static class HostExtensions
    {
        public static async Task<IHost> MigrateSeedData(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            try
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<MasterDbContext>();
                await context.Database.MigrateAsync();

                var mediator = services.GetRequiredService<IMediator>();
                await mediator.Send(new SeedSampleDataCommand(), CancellationToken.None);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating or initializing the database.");
            }

            return host;
        }
    }
}