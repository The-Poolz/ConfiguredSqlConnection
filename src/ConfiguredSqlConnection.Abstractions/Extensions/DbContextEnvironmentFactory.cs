using EnvironmentManager.Static;
using Microsoft.EntityFrameworkCore;

namespace ConfiguredSqlConnection.Abstractions.Extensions;

public class DbContextEnvironmentFactory<TContext>(DbContextOptionsBuilderFactory<TContext> optionsBuilderFactory)
    : DbContextFactory<TContext>(optionsBuilderFactory)
    where TContext : DbContext
{
    public virtual TContext CreateFromEnvironment()
    {
        var dbMode = EnvManager.GetRequired<ContextOption>("CONFIGUREDSQLCONNECTION_DB_MODE");
        var dbName = EnvManager.Get<string>("CONFIGUREDSQLCONNECTION_DB_NAME");
        return Create(dbMode, dbName);
    }
}
