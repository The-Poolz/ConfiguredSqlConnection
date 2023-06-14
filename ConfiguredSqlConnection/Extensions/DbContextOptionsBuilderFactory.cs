using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace ConfiguredSqlConnection.Extensions;

public class DbContextOptionsBuilderFactory<TContext> where TContext : DbContext
{
    protected readonly DbContextOptionsBuilder<TContext> optionsBuilder;

    public DbContextOptionsBuilderFactory()
    {
        optionsBuilder = new DbContextOptionsBuilder<TContext>();
    }

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

    protected virtual void ConfigureProdContext()
    {
        optionsBuilder.UseSqlServer(ConnectionStringFactory.GetProdConnection());
    }

    protected virtual void ConfigureStagingContext(string? dbName)
    {
        optionsBuilder.UseSqlServer(ConnectionStringFactory.GetStagingConnection(dbName));
    }

    protected virtual void ConfigureInMemoryContext(string? dbName)
    {
        optionsBuilder.UseInMemoryDatabase(CheckDbName(dbName));
    }

    private static string CheckDbName(string? dbName) =>
        !string.IsNullOrWhiteSpace(dbName) ? dbName : throw new ArgumentNullException(nameof(dbName));
}
