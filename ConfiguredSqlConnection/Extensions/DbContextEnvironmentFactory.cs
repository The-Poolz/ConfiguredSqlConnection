using EnvironmentManager;
using Microsoft.EntityFrameworkCore;

namespace ConfiguredSqlConnection.Extensions;

public class DbContextEnvironmentFactory<TContext> : DbContextFactory<TContext> where TContext : DbContext
{
    private readonly ContextOption dbMode;
    private readonly string dbName;

    public DbContextEnvironmentFactory()
        : this(new DbContextOptionsBuilderFactory<TContext>())
    { }

    public DbContextEnvironmentFactory(DbContextOptionsBuilderFactory<TContext> optionsBuilderFactory)
        : base(optionsBuilderFactory)
    {
        var envManager = new EnvManager();
        dbMode = envManager.GetEnvironmentValue<ContextOption>("CONFIGUREDSQLCONNECTION_DB_MODE", true);
        dbName = envManager.GetEnvironmentValue<string>("CONFIGUREDSQLCONNECTION_DB_NAME");
    }

    public virtual TContext CreateFromEnvironment() =>
        Create(dbMode, dbName);
}
