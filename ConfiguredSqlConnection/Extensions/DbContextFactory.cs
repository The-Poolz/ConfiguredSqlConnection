using Microsoft.EntityFrameworkCore;

namespace ConfiguredSqlConnection.Extensions;

public class DbContextFactory<TContext> where TContext : DbContext
{
    private readonly DbContextOptionsBuilderFactory<TContext> optionsBuilderFactory;

    public DbContextFactory()
        : this(new DbContextOptionsBuilderFactory<TContext>())
    { }

    public DbContextFactory(DbContextOptionsBuilderFactory<TContext> optionsBuilderFactory)
    {
        this.optionsBuilderFactory = optionsBuilderFactory;
    }

    public virtual TContext Create(ContextOption option, string? dbName = null)
    {
        var optionsBuilder = optionsBuilderFactory.Create(option, dbName);

        return (TContext)Activator.CreateInstance(typeof(TContext), optionsBuilder.Options)!;
    }
}
