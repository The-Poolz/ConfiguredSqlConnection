using Xunit;
using FluentAssertions;
using ConfiguredSqlConnection.PostgresSql.Extensions;
using ConfiguredSqlConnection.Abstractions.Extensions;

namespace ConfiguredSqlConnection.PostgresSql.Tests.Extensions;

public class PostgresSqlDbContextOptionsBuilderFactoryTests
{
    [Fact]
    public void Create_Staging_ConfiguresProvider()
    {
        var dbName = "TestDb";
        var connectionString = "Host=localhost;Database=db;Username=user;Password=pass";
        var appSettings = $"{{\"ConnectionStrings\":{{\"{dbName}\":\"{connectionString}\"}}}}";
        File.WriteAllText("appsettings.json", appSettings);

        try
        {
            var factory = new PostgresSqlDbContextOptionsBuilderFactory<TestDbContext>();

            var optionsBuilder = factory.Create(ContextOption.Staging, dbName);

            using var context = new TestDbContext(optionsBuilder.Options);
            context.Database.ProviderName.Should().Be("Npgsql.EntityFrameworkCore.PostgreSQL");
        }
        finally
        {
            File.Delete("appsettings.json");
        }
    }
}