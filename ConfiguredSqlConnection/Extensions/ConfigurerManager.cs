using SecretsManager;
using EnvironmentManager;
using Microsoft.EntityFrameworkCore;

namespace ConfiguredSqlConnection.Extensions;

public static class ConfigurerManager
{
    public static void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var actionConnection = EnvManager.GetEnvironmentValue<string>("CONFIGUREDSQLCONNECTION_ACTION_CONNECTION");
            if (!string.IsNullOrEmpty(actionConnection))
            {
                optionsBuilder.UseSqlServer(actionConnection);
            }

            var secretValue = EnvManager.GetEnvironmentValue<string>("CONFIGUREDSQLCONNECTION_SECRET_NAME_OF_CONNECTION", true);

            var connectionString = new SecretManager().GetSecretValue(secretValue, "connectionString");

            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
