using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ConfiguredSqlConnection.PostgresSql.Extensions;

namespace ConfiguredSqlConnection.PostgresSql.Tests.Extensions;

public class PostgresSqlDbContextOptionsBuilderExtensionsTests
{
    private class TestDbContext(DbContextOptions options) : DbContext(options);

    [Fact]
    public void ConfigureFromActionConnection_WithEnvVar_ConfiguresProvider()
    {
        var envVarName = "CONFIGUREDSQLCONNECTION_ACTION_CONNECTION";
        var connectionString = "Host=localhost;Database=db;Username=user;Password=pass";
        Environment.SetEnvironmentVariable(envVarName, connectionString);

        var builder = new DbContextOptionsBuilder<TestDbContext>();
        builder.ConfigureFromActionConnection(envVarName: envVarName);

        using var context = new TestDbContext(builder.Options);
        context.Database.ProviderName.Should().Be("Npgsql.EntityFrameworkCore.PostgreSQL");

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