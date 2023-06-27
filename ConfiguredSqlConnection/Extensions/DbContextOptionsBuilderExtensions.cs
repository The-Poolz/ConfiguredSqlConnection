using SecretsManager;
using EnvironmentManager;
using Microsoft.EntityFrameworkCore;

namespace ConfiguredSqlConnection.Extensions;

public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder OnConfiguring(this DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var actionConnection = EnvManager.GetEnvironmentValue<string>("CONFIGUREDSQLCONNECTION_ACTION_CONNECTION");
            if (!string.IsNullOrEmpty(actionConnection))
            {
                optionsBuilder.UseSqlServer(actionConnection);
                return optionsBuilder;
            }

            var secretValue = EnvManager.GetEnvironmentValue<string>("CONFIGUREDSQLCONNECTION_SECRET_NAME_OF_CONNECTION", true);
            var connectionString = new SecretManager().GetSecretValue(secretValue, "connectionString");

            optionsBuilder.UseSqlServer(connectionString);
        }

        return optionsBuilder;
    }
}
