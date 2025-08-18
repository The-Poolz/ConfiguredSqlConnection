using Xunit;
using FluentAssertions;
using ConfiguredSqlConnection.PostgresSql.Tests;
using ConfiguredSqlConnection.SqlServer.Extensions;
using ConfiguredSqlConnection.Abstractions.Extensions;

namespace ConfiguredSqlConnection.SqlServer.Tests.Extensions;

public class SqlServerDbContextOptionsBuilderFactoryTests
{
    [Fact]
    public void Create_Staging_ConfiguresProvider()
    {
        var dbName = "TestDb";
        var connectionString = @"Server=(localdb)\\mssqllocaldb;Database=ActionDb;Trusted_Connection=True";
        var appSettings = $"{{\"ConnectionStrings\":{{\"{dbName}\":\"{connectionString}\"}}}}";
        File.WriteAllText("appsettings.json", appSettings);

        try
        {
            var factory = new SqlServerDbContextOptionsBuilderFactory<TestDbContext>();

            var optionsBuilder = factory.Create(ContextOption.Staging, dbName);

            using var context = new TestDbContext(optionsBuilder.Options);
            context.Database.ProviderName.Should().Be("Microsoft.EntityFrameworkCore.SqlServer");
        }
        finally
        {
            File.Delete("appsettings.json");
        }
    }
}