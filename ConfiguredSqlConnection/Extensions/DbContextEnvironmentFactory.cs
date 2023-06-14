using EnvironmentManager;
using Microsoft.EntityFrameworkCore;

namespace ConfiguredSqlConnection.Extensions;

public class DbContextEnvironmentFactory<TContext> : DbContextFactory<TContext> where TContext : DbContext
{
    private readonly string dbMode;
    private readonly string dbName;

    public DbContextEnvironmentFactory()
        : this(new DbContextOptionsBuilderFactory<TContext>())
    { }

    public DbContextEnvironmentFactory(DbContextOptionsBuilderFactory<TContext> optionsBuilderFactory)
        : base(optionsBuilderFactory)
    {
        dbMode = EnvManager.GetEnvironmentValue<string>("CONFIGUREDSQLCONNECTION_DB_MODE", true);
        dbName = EnvManager.GetEnvironmentValue<string>("CONFIGUREDSQLCONNECTION_DB_NAME");
    }

    public virtual TContext CreateFromEnvironment()
    {
        if (!Enum.TryParse(typeof(ContextOption), dbMode, true, out var optionObj) || optionObj is not ContextOption option)
        {
            throw new ArgumentException($"Invalid value for environment variable 'CONFIGUREDSQLCONNECTION_DB_MODE': {dbMode}");
        }

        return Create(option, dbName);
    }
}
