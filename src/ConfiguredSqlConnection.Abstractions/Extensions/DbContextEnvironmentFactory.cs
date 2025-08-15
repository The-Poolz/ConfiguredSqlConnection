using EnvironmentManager.Static;
using Microsoft.EntityFrameworkCore;

namespace ConfiguredSqlConnection.Abstractions.Extensions;

public class DbContextEnvironmentFactory<TContext>(DbContextOptionsBuilderFactory<TContext> optionsBuilderFactory)
    : DbContextFactory<TContext>(optionsBuilderFactory)
    where TContext : DbContext
{
    private readonly ContextOption dbMode = EnvManager.Get<ContextOption>("CONFIGUREDSQLCONNECTION_DB_MODE", true);
    private readonly string dbName = EnvManager.Get<string>("CONFIGUREDSQLCONNECTION_DB_NAME");

    public virtual TContext CreateFromEnvironment() =>
        Create(dbMode, dbName);
}
