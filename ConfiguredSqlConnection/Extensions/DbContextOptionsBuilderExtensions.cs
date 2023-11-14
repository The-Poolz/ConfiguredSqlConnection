using SecretsManager;
using EnvironmentManager;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Internal;

namespace ConfiguredSqlConnection.Extensions;

public static class DbContextOptionsBuilderExtensions
{
    private static readonly EnvManager envManager = new();

    public static DbContextOptionsBuilder ConfigureFromActionConnection(this DbContextOptionsBuilder optionsBuilder, string? migrationsAssembly = null)
    {
        if (optionsBuilder.IsConfigured) return optionsBuilder;
        var connectionString = envManager.GetEnvironmentValue<string>("CONFIGUREDSQLCONNECTION_ACTION_CONNECTION");
        if (string.IsNullOrEmpty(connectionString)) return optionsBuilder;

        if (string.IsNullOrWhiteSpace(migrationsAssembly))
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
        else
        {
            optionsBuilder.UseSqlServer(connectionString, options => options.MigrationsAssembly(migrationsAssembly));
        }

        return optionsBuilder;
    }

    public static DbContextOptionsBuilder ConfigureFromSecretConnection(this DbContextOptionsBuilder optionsBuilder, string? migrationsAssembly = null)
    {
        if (optionsBuilder.IsConfigured) return optionsBuilder;
        var secretValue = envManager.GetEnvironmentValue<string>("CONFIGUREDSQLCONNECTION_SECRET_NAME_OF_CONNECTION", true);
        var connectionString = new SecretManager().GetSecretValue(secretValue, "connectionString");
        if (string.IsNullOrEmpty(connectionString)) return optionsBuilder;

        if (string.IsNullOrWhiteSpace(migrationsAssembly))
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
        else
        {
            optionsBuilder.UseSqlServer(connectionString, options => options.MigrationsAssembly(migrationsAssembly));
        }

        return optionsBuilder;
    }
}
