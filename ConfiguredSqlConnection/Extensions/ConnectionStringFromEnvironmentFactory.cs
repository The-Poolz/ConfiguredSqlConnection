using EnvironmentManager;

namespace ConfiguredSqlConnection.Extensions;

public static class ConnectionStringFromEnvironmentFactory
{
    private static ContextOption DbMode =>
        EnvManager.GetEnvironmentValue<ContextOption>("CONFIGUREDSQLCONNECTION_DB_MODE", true);
    private static string DbName =>
        EnvManager.GetEnvironmentValue<string>("CONFIGUREDSQLCONNECTION_DB_NAME");

    public static string GetConnectionFromEnvironment() =>
        ConnectionStringFactory.GetConnection(DbMode, DbName);
}
