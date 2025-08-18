using Microsoft.EntityFrameworkCore;

namespace ConfiguredSqlConnection.Abstractions.Extensions;

public class DbContextFactory<TContext>(DbContextOptionsBuilderFactory<TContext> optionsBuilderFactory)
    where TContext : DbContext
{
    public virtual TContext Create(ContextOption option, string? dbName = null)
    {
        var optionsBuilder = optionsBuilderFactory.Create(option, dbName);

        return (TContext)Activator.CreateInstance(typeof(TContext), optionsBuilder.Options)!;
    }
}
