using Confluent.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Slave2.Controllers;
using Slave2.HostedService;
using Slave2.Persistence;
using VF.Logging.OpenTelemetry;
using VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Extensions;
using System;
using System.Threading.Tasks;

namespace Slave2
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<Slave2DbContext>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString("Postgres")))
                .AddHostedService<KafkaConsumerHostedService>()
                .Configure<ServicesUrlsOptions>(Configuration.GetSection("ServicesUrlsOptions"))
                .AddCors()
                .AddHttpClient<Slave2Controller>();

            services.AddOpenTelemetry(Configuration, "nslave2", b =>
                b.AddOtelKafkaConsumer(Configuration.GetSection("KafkaConsumer").Get<ConsumerConfig>(),
                    Configuration.GetSection("KafkaConsumer:Tags").Get<string[]>()))
                .AddHttpContextAccessor()
                .AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(c => c.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseRouting();

            app.Use(async (_, next) =>
            {
                var rnd = new Random();
                var number = rnd.NextDouble() + 1;
                var sec = Math.Floor(number * 1000);
                await Task.Delay(Convert.ToInt32(sec));
                await next.Invoke();
            });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
