using EnvironmentManager;
using Microsoft.EntityFrameworkCore;
using SecretsManager;

namespace ConfiguredSqlConnection;

public class DataBaseContext : DbContext, IAsyncDisposable, IDisposable
{
    public DataBaseContext() { }
    public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var actionConnection = EnvManager.GetEnvironmentValue<string>("ACTION_CONNECTION");
        if (!string.IsNullOrEmpty(actionConnection))
        {
            optionsBuilder.UseSqlServer(actionConnection);
        }
        if (!optionsBuilder.IsConfigured)
        {
            var secretValue = EnvManager.GetEnvironmentValue<string>("SECRET_NAME_OF_CONNECTION", true);

            var connectionString = new SecretManager().GetSecretValue(secretValue, "connectionString");

            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
