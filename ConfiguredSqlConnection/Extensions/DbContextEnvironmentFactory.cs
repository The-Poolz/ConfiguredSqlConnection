using EnvironmentManager;

namespace ConfiguredSqlConnection.Extensions;

public class DbContextEnvironmentFactory : DbContextFactory
{
    private readonly string dbMode;
    private readonly string dbName;

    public DbContextEnvironmentFactory()
        : this(new DbContextOptionsBuilderFactory<DataBaseContext>())
    { }

    public DbContextEnvironmentFactory(DbContextOptionsBuilderFactory<DataBaseContext> optionsBuilderFactory)
        : base(optionsBuilderFactory)
    {
        dbMode = EnvManager.GetEnvironmentValue<string>("DB_MODE", true);
        dbName = EnvManager.GetEnvironmentValue<string>("DB_NAME");
    }

    public virtual DataBaseContext CreateFromEnvironment()
    {
        if (!Enum.TryParse(typeof(ContextOption), dbMode, true, out var optionObj) || !(optionObj is ContextOption option))
        {
            throw new ArgumentException($"Invalid value for environment variable 'DB_MODE': {dbMode}");
        }

        return Create(option, dbName);
    }
}
