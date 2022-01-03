using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedisCache;
using VF.Logging.OpenTelemetry;
using System;
using System.Threading.Tasks;

namespace Slave1
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) => services.AddCors()
                .AddRedisCacheService(Configuration.GetSection("RedisConnection").Get<string>())
                .AddOpenTelemetry(Configuration, "nslave1")
                .AddControllers();

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(c => c.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseHttpsRedirection();

            app.UseRouting();

            app.Use(async (_, next) =>
            {
                var rnd = new Random();
                var number = rnd.NextDouble();
                var sec = Math.Floor(number * 1000);
                await Task.Delay(Convert.ToInt32(sec));
                await next.Invoke();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
