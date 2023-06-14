using SecretsManager;
using EnvironmentManager;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ConfiguredSqlConnection.Extensions;

public static class ConnectionStringFactory
{
    public static string GetConnection(ContextOption option, string? dbName = null) =>
        option switch
        {
            ContextOption.Prod => GetProdConnection(),
            ContextOption.Staging => GetStagingConnection(dbName),
            _ => throw new InvalidEnumArgumentException(nameof(option), (int)option, typeof(ContextOption)),
        };

    public static string GetProdConnection()
    {
        var secretValue = EnvManager.GetEnvironmentValue<string>("CONFIGUREDSQLCONNECTION_SECRET_NAME_OF_CONNECTION", true);

        return new SecretManager().GetSecretValue(secretValue, "connectionString");
    }

    public static string GetStagingConnection(string? dbName)
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