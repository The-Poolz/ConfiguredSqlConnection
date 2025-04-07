using SecretsManager;
using EnvironmentManager.Static;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ConfiguredSqlConnection.Extensions;

public static class DbContextOptionsBuilderExtensions
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

        optionsBuilder.UseSqlServer(connectionString, ConfigureSqlServerOptionsAction(migrationsAssembly));

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

        optionsBuilder.UseSqlServer(connectionString, ConfigureSqlServerOptionsAction(migrationsAssembly));

        return optionsBuilder;
    }

    private static Action<SqlServerDbContextOptionsBuilder>? ConfigureSqlServerOptionsAction(string? migrationsAssembly = null) =>
        !string.IsNullOrWhiteSpace(migrationsAssembly)
            ? options => options.MigrationsAssembly(migrationsAssembly)
            : null;
}
