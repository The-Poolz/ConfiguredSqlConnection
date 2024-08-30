using EnvironmentManager.Static;

namespace ConfiguredSqlConnection.Extensions;

public static class ConnectionStringFromEnvironmentFactory
{
    private static ContextOption DbMode =>
        EnvManager.Get<ContextOption>("CONFIGUREDSQLCONNECTION_DB_MODE", true);
    private static string DbName =>
        EnvManager.Get<string>("CONFIGUREDSQLCONNECTION_DB_NAME");

    public static string GetConnectionFromEnvironment() =>
        ConnectionStringFactory.GetConnection(DbMode, DbName);
}