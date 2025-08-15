using Microsoft.EntityFrameworkCore;
using ConfiguredSqlConnection.Extensions;

namespace ConfiguredSqlConnection.PostgresSql.Extensions;

public class PostgresSqlDbContextOptionsBuilderFactory<TContext> : DbContextOptionsBuilderFactory<TContext>
    where TContext : DbContext
{
    protected override void ConfigureProdContext()
    {
        optionsBuilder.UseNpgsql(ConnectionStringFactory.GetConnectionFromSecret());
    }

    protected override void ConfigureStagingContext(string? dbName)
    {
        optionsBuilder.UseNpgsql(ConnectionStringFactory.GetConnectionFromConfiguration(dbName));
    }
}