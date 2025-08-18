using Xunit;
using FluentAssertions;
using System.ComponentModel;
using ConfiguredSqlConnection.Abstractions.Extensions;

namespace ConfiguredSqlConnection.Abstractions.Tests.Extensions;

public class ConnectionStringFactoryTests
{
    [Fact]
    public void GetConnectionFromConfiguration_ReturnsConnectionString()
    {
        var expected = "Server=.;Database=TestDb;";
        var path = Path.Combine(Environment.CurrentDirectory, "appsettings.json");
        File.WriteAllText(path, $"{{\"ConnectionStrings\":{{\"TestDb\":\"{expected}\"}}}}");
        try
        {
            var actual = ConnectionStringFactory.GetConnectionFromConfiguration("TestDb");
            actual.Should().Be(expected);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void GetConnectionFromConfiguration_MissingDbName_Throws()
    {
        Action act = () => ConnectionStringFactory.GetConnectionFromConfiguration(null);
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("dbName");
    }

    [Fact]
    public void GetConnection_InvalidOption_Throws()
    {
        var invalid = (ContextOption)42;
        Action act = () => ConnectionStringFactory.GetConnection(invalid);
        act.Should().Throw<InvalidEnumArgumentException>();
    }
}