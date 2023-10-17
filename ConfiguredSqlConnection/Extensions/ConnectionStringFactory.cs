using SecretsManager;
using EnvironmentManager;
using System.ComponentModel;
using Microsoft.Extensions.Configuration;

namespace ConfiguredSqlConnection.Extensions;

public static class ConnectionStringFactory
{
    public static string GetConnection(ContextOption option, string? dbName = null) =>
        option switch
        {
            ContextOption.Prod => GetConnectionFromSecret(),
            ContextOption.Staging => GetConnectionFromConfiguration(dbName),
            _ => throw new InvalidEnumArgumentException(nameof(option), (int)option, typeof(ContextOption)),
        };

    public static string GetConnectionFromSecret()
    {
        var secretValue = new EnvManager().GetEnvironmentValue<string>("CONFIGUREDSQLCONNECTION_SECRET_NAME_OF_CONNECTION", true);

        return new SecretManager().GetSecretValue(secretValue, "connectionString");
    }

    public static string GetConnectionFromConfiguration(string? dbName)
    {
        if (string.IsNullOrWhiteSpace(dbName))
            throw new ArgumentNullException(nameof(dbName));

        var config = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = config.GetConnectionString(dbName);
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException($"Connection string for database '{dbName}' not found in configuration.");

        return connectionString;
    }
}