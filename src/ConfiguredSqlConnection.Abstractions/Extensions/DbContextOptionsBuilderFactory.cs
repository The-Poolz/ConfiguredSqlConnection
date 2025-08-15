using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace ConfiguredSqlConnection.Abstractions.Extensions;

public abstract class DbContextOptionsBuilderFactory<TContext> where TContext : DbContext
{
    protected readonly DbContextOptionsBuilder<TContext> optionsBuilder = new();

    public virtual DbContextOptionsBuilder<TContext> Create(ContextOption option, string? dbName = null)
    {
        switch (option)
        {
            case ContextOption.Prod:
                ConfigureProdContext();
                break;
            case ContextOption.Staging:
                ConfigureStagingContext(dbName);
                break;
            case ContextOption.InMemory:
                ConfigureInMemoryContext(dbName);
                break;
            default:
                throw new InvalidEnumArgumentException(nameof(option), (int)option, typeof(ContextOption));
        }

        return optionsBuilder;
    }

    protected abstract void ConfigureProdContext();

    protected abstract void ConfigureStagingContext(string? dbName);

    protected virtual void ConfigureInMemoryContext(string? dbName)
    {
        ArgumentException.ThrowIfNullOrEmpty(dbName, nameof(dbName));
        optionsBuilder.UseInMemoryDatabase(dbName);
    }
}
