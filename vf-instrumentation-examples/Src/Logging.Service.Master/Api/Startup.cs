using System.Text;
using Application;
using Application.Common.Decorators;
using Confluent.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Infrastructure;
using Persistence;
using RedisCache;
using VF.Logging.OpenTelemetry;
using VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Extensions;

namespace Master
{
    public class Startup
    {
        private IServiceCollection _services;

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var config = Configuration.GetSection("KafkaProducer").Get<ProducerConfig>();

            services
                .AddInfrastructure(Configuration, "ServicesUrls")
                .AddPersistence(Configuration)
                .AddRedisCacheService(Configuration.GetValue<string>("RedisConnection"))
                .AddApplication(Configuration)
                .AddOpenTelemetry(Configuration,
                    "nmaster", 
                    builder => builder.AddOtelKafkaProducer(config, new []{"body"}))
                .AddDecorators();

            services.AddHealthChecks()
                .AddDbContextCheck<MasterDbContext>();

            services.AddCors();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "vf-instrumentation-examples", Version = "v1" });
            });

            _services = services;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
                RegisteredServicesPage(app);
            }

            //app.UseAllElasticApm(Configuration);

            app.UseHealthChecks("/health");

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TelemetryDemoApi v1"));

            app.UseCors(c => c.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseHttpsRedirection();

            app.UseRouting();

            // app.UseCustomExceptionHandler();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/app", async context => { await context.Response.WriteAsync("HI.It is working"); });
            });
        }

        private void RegisteredServicesPage(IApplicationBuilder app)
        {
            app.Map("/services", builder => builder.Run(async context =>
            {
                var sb = new StringBuilder();
                sb.Append("<h1>Registered Services</h1>");
                sb.Append("<table><thead>");
                sb.Append("<tr><th>Type</th><th>Lifetime</th><th>Instance</th></tr>");
                sb.Append("</thead><tbody>");
                foreach (var svc in _services)
                {
                    sb.Append("<tr>");
                    sb.Append($"<td>{svc.ServiceType.FullName}</td>");
                    sb.Append($"<td>{svc.Lifetime}</td>");
                    sb.Append($"<td>{svc.ImplementationType?.FullName}</td>");
                    sb.Append("</tr>");
                }

                sb.Append("</tbody></table>");
                await context.Response.WriteAsync(sb.ToString());
            }));
        }
    }
}