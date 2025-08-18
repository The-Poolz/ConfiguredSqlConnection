using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ConfiguredSqlConnection.SqlServer.Extensions;
namespace ConfiguredSqlConnection.SqlServer.Tests.Extensions;

public class SqlServerDbContextOptionsBuilderExtensionsTests
{
    private class TestDbContext(DbContextOptions options) : DbContext(options);

    [Fact]
    public void ConfigureFromActionConnection_WithEnvVar_ConfiguresProvider()
    {
        var envVarName = "CONFIGUREDSQLCONNECTION_ACTION_CONNECTION";
        var connectionString = @"Server=(localdb)\\mssqllocaldb;Database=ActionDb;Trusted_Connection=True";
        Environment.SetEnvironmentVariable(envVarName, connectionString);

        var builder = new DbContextOptionsBuilder<TestDbContext>();
        builder.ConfigureFromActionConnection(envVarName: envVarName);

        using var context = new TestDbContext(builder.Options);
        context.Database.ProviderName.Should().Be("Microsoft.EntityFrameworkCore.SqlServer");

        Environment.SetEnvironmentVariable(envVarName, null);
    }

    [Fact]
    public void ConfigureFromActionConnection_NoEnvVar_DoesNotConfigure()
    {
        var envVarName = "CONFIGUREDSQLCONNECTION_ACTION_CONNECTION";
        Environment.SetEnvironmentVariable(envVarName, null);

        var builder = new DbContextOptionsBuilder<TestDbContext>();
        builder.ConfigureFromActionConnection(envVarName: envVarName);

        builder.IsConfigured.Should().BeFalse();
    }

    [Fact]
    public void ConfigureFromSecretConnection_NoEnvVar_Throws()
    {
        var envVarName = "CONFIGUREDSQLCONNECTION_SECRET_NAME_OF_CONNECTION";
        Environment.SetEnvironmentVariable(envVarName, null);

        var builder = new DbContextOptionsBuilder<TestDbContext>();

        var act = () => builder.ConfigureFromSecretConnection(envVarName: envVarName);
        act.Should().Throw<ArgumentNullException>();
    }
}