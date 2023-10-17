using EnvironmentManager;

namespace ConfiguredSqlConnection.Extensions;

public static class ConnectionStringFromEnvironmentFactory
{
    private static readonly EnvManager envManager = new();
    private static ContextOption DbMode =>
        envManager.GetEnvironmentValue<ContextOption>("CONFIGUREDSQLCONNECTION_DB_MODE", true);
    private static string DbName =>
        envManager.GetEnvironmentValue<string>("CONFIGUREDSQLCONNECTION_DB_NAME");

    public static string GetConnectionFromEnvironment() =>
        ConnectionStringFactory.GetConnection(DbMode, DbName);
}
