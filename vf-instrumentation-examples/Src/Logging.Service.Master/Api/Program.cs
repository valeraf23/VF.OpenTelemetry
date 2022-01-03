using System.Threading.Tasks;
using Master.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using VF.Logging.OpenTelemetry.Logs.Serilog.Sinks.File;

namespace Master
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
            var host = await CreateHostBuilder(args)
                       .Build()
                       .MigrateSeedData();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddVfLog(context.Configuration);
                    logging.AddOpenTelemetry(options =>
                    {
                        options.AddConsoleExporter();
                    });
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
