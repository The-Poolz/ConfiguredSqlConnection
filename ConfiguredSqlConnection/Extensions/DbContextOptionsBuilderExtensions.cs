using SecretsManager;
using EnvironmentManager;
using Microsoft.EntityFrameworkCore;

namespace ConfiguredSqlConnection.Extensions;

public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder ConfigureFromActionConnection(this DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return optionsBuilder;

        var connectionString = EnvManager.GetEnvironmentValue<string>("CONFIGUREDSQLCONNECTION_ACTION_CONNECTION");
        if (!string.IsNullOrEmpty(connectionString))
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        return optionsBuilder;
    }

    public static DbContextOptionsBuilder ConfigureFromSecretConnection(this DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return optionsBuilder;

        var secretValue = EnvManager.GetEnvironmentValue<string>("CONFIGUREDSQLCONNECTION_SECRET_NAME_OF_CONNECTION", true);
        var connectionString = new SecretManager().GetSecretValue(secretValue, "connectionString");
        if (!string.IsNullOrEmpty(connectionString))
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        return optionsBuilder;
    }
}
