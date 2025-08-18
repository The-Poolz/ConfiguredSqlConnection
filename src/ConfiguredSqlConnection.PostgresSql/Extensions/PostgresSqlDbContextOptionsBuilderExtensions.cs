using SecretsManager;
using EnvironmentManager.Static;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace ConfiguredSqlConnection.PostgresSql.Extensions;

public static class PostgresSqlDbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder ConfigureFromActionConnection(
        this DbContextOptionsBuilder optionsBuilder,
        string? migrationsAssembly = null,
        string envVarName = "CONFIGUREDSQLCONNECTION_ACTION_CONNECTION"
    )
    {
        if (optionsBuilder.IsConfigured) return optionsBuilder;
        var connectionString = EnvManager.Get<string>(envVarName);
        if (string.IsNullOrEmpty(connectionString)) return optionsBuilder;

        optionsBuilder.UseNpgsql(connectionString, ConfigureNpgsqlOptionsAction(migrationsAssembly));

        return optionsBuilder;
    }

    public static DbContextOptionsBuilder ConfigureFromSecretConnection(
        this DbContextOptionsBuilder optionsBuilder,
        string? migrationsAssembly = null,
        string envVarName = "CONFIGUREDSQLCONNECTION_SECRET_NAME_OF_CONNECTION"
    )
    {
        if (optionsBuilder.IsConfigured) return optionsBuilder;
        var secretValue = EnvManager.GetRequired<string>(envVarName);
        var connectionString = new SecretManager().GetSecretValue(secretValue, "connectionString");
        if (string.IsNullOrEmpty(connectionString)) return optionsBuilder;

        optionsBuilder.UseNpgsql(connectionString, ConfigureNpgsqlOptionsAction(migrationsAssembly));

        return optionsBuilder;
    }

    private static Action<NpgsqlDbContextOptionsBuilder>? ConfigureNpgsqlOptionsAction(string? migrationsAssembly = null) =>
        !string.IsNullOrWhiteSpace(migrationsAssembly)
            ? options => options.MigrationsAssembly(migrationsAssembly)
            : null;
}