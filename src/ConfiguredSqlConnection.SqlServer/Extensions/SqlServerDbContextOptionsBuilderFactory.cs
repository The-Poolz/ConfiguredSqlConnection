using Microsoft.EntityFrameworkCore;
using ConfiguredSqlConnection.Abstractions.Extensions;

namespace ConfiguredSqlConnection.SqlServer.Extensions;

public class SqlServerDbContextOptionsBuilderFactory<TContext> : DbContextOptionsBuilderFactory<TContext>
    where TContext : DbContext
{
    protected override void ConfigureProdContext()
    {
        optionsBuilder.UseSqlServer(ConnectionStringFactory.GetConnectionFromSecret());
    }

    protected override void ConfigureStagingContext(string? dbName)
    {
        optionsBuilder.UseSqlServer(ConnectionStringFactory.GetConnectionFromConfiguration(dbName));
    }
}