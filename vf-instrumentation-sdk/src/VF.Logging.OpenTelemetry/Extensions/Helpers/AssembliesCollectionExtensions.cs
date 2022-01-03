using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VF.Logging.OpenTelemetry.Extensions.Helpers
{
    internal static class AssembliesCollectionExtensions
    {
        internal static IEnumerable<Assembly> GetUniqAssemblies(this IEnumerable<Assembly> assemblies)
        {
            return assemblies.GroupBy(a => a.FullName).Select(g => g.First());
        }
    }
}