using EnvironmentManager;
using Microsoft.EntityFrameworkCore;
using SecretsManager;

namespace ConfiguredSqlConnection;

public class DataBaseContext : DbContext, IAsyncDisposable, IDisposable
{
    public DataBaseContext() { }
    public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options) { }

    
}
