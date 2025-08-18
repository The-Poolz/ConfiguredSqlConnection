using Xunit;
using FluentAssertions;
using ConfiguredSqlConnection.Abstractions.Extensions;

namespace ConfiguredSqlConnection.Abstractions.Tests.Extensions;

public class ConnectionStringFromEnvironmentFactoryTests
{
    [Fact]
    public void GetConnectionFromEnvironment_ReturnsConnectionString()
    {
        var expected = "Server=.;Database=EnvDb;";
        Environment.SetEnvironmentVariable("CONFIGUREDSQLCONNECTION_DB_MODE", ContextOption.Staging.ToString());
        Environment.SetEnvironmentVariable("CONFIGUREDSQLCONNECTION_DB_NAME", "EnvDb");
        var path = Path.Combine(Environment.CurrentDirectory, "appsettings.json");
        File.WriteAllText(path, $"{{\"ConnectionStrings\":{{\"EnvDb\":\"{expected}\"}}}}");
        try
        {
            var actual = ConnectionStringFromEnvironmentFactory.GetConnectionFromEnvironment();
            actual.Should().Be(expected);
        }
        finally
        {
            File.Delete(path);
        }
    }
}