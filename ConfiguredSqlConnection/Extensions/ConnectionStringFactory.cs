using SecretsManager;
using System.ComponentModel;
using EnvironmentManager.Static;
using Microsoft.Extensions.Configuration;

namespace ConfiguredSqlConnection.Extensions;

public static class ConnectionStringFactory
{
    public static string GetConnection(ContextOption option, string? dbName = null, string envVarName = "CONFIGUREDSQLCONNECTION_SECRET_NAME_OF_CONNECTION") =>
        option switch
        {
            ContextOption.Prod => GetConnectionFromSecret(envVarName),
            ContextOption.Staging => GetConnectionFromConfiguration(dbName),
            _ => throw new InvalidEnumArgumentException(nameof(option), (int)option, typeof(ContextOption)),
        };

    public static string GetConnectionFromSecret(string envVarName = "CONFIGUREDSQLCONNECTION_SECRET_NAME_OF_CONNECTION")
    {
        var secretValue = EnvManager.GetRequired(envVarName);

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