namespace ConfiguredSqlConnection.Extensions;

public class DbContextFactory
{
    private readonly DbContextOptionsBuilderFactory<DataBaseContext> optionsBuilderFactory;

    public DbContextFactory()
        : this(new DbContextOptionsBuilderFactory<DataBaseContext>())
    { }

    public DbContextFactory(DbContextOptionsBuilderFactory<DataBaseContext> optionsBuilderFactory)
    {
        this.optionsBuilderFactory = optionsBuilderFactory;
    }

    public virtual DataBaseContext Create(ContextOption option, string? dbName = null)
    {
        var optionsBuilder = optionsBuilderFactory.Create(option, dbName);

        return new DataBaseContext(optionsBuilder.Options);
    }
}
