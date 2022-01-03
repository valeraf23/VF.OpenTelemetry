using Microsoft.Extensions.DependencyInjection;
using VF.Logging.OpenTelemetry.TraceInstrumentation;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VF.Logging.OpenTelemetry.Extensions.Helpers;

namespace VF.Logging.OpenTelemetry.Extensions.ServiceCollectionExtensions
{
    internal static class TraceInstrumentationServiceCollectionExtensions
    {

        private static IServiceCollection AddInstrumentation(this IServiceCollection services, IEnumerable<string> keys, IEnumerable<Assembly> assemblies) =>
            services.Scan(s =>
                s.FromAssemblies(assemblies.GetUniqAssemblies())
                    .AddClasses(c =>
                        c.AssignableTo<IInstrumentation>().Where(t => keys.Any(k => t.Name.StartsWith(k))))
                    .AsImplementedInterfaces()
                    .WithSingletonLifetime());

        internal static IServiceCollection AddInstrumentation(this IServiceCollection services, IEnumerable<string> keys, Assembly assemble, params Assembly[] assemblies)
        {
            var assembleList = new List<Assembly>(assemblies) { assemble, Assembly.GetAssembly(typeof(IInstrumentation))! };
            return AddInstrumentation(services, keys, assembleList);
        }

        internal static IServiceCollection AddInstrumentation(this IServiceCollection services, IEnumerable<string> keys) =>
            AddInstrumentation(services, keys, new[] { Assembly.GetAssembly(typeof(IInstrumentation))! });

        internal static IServiceCollection AddService(this IServiceCollection services) => services;
    }
}