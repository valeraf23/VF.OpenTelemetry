using System.Reflection;
using FluentAssertions;
using VF.Logging.OpenTelemetry.Extensions.Helpers;
using Xunit;

namespace VF.Logging.OpenTelemetry.UnitTests
{
    public class AssembliesCollectionExtensionsTests
    {
        [Fact]
        public void ShouldGetUniqAssemblies()
        {
            var assemblies = new[]{Assembly.GetExecutingAssembly(), Assembly.GetExecutingAssembly()};
            assemblies.GetUniqAssemblies().Should().HaveCount(1);
        }
    }
}