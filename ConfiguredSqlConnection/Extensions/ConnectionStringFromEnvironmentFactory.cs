using EnvironmentManager;

namespace ConfiguredSqlConnection.Extensions;

public static class ConnectionStringFromEnvironmentFactory
{
    private static string DbMode =>
        EnvManager.GetEnvironmentValue<string>("CONFIGUREDSQLCONNECTION_DB_MODE", true);
    private static string DbName =>
        EnvManager.GetEnvironmentValue<string>("CONFIGUREDSQLCONNECTION_DB_NAME");

    public static string GetConnectionFromEnvironment()
    {
        if (!Enum.TryParse(typeof(ContextOption), DbMode, true, out var optionObj) || optionObj is not ContextOption option)
        {
            throw new ArgumentException($"Invalid value for environment variable 'CONFIGUREDSQLCONNECTION_DB_MODE': {DbMode}");
        }

        return ConnectionStringFactory.GetConnection(option, DbName);
    }
}
