using EnvironmentManager;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SecretsManager;
using System.ComponentModel;

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
        var secretValue = EnvManager.GetEnvironmentValue<string>("CONFIGUREDSQLCONNECTION_SECRET_NAME_OF_CONNECTION", true);

        var connectionString = new SecretManager().GetSecretValue(secretValue, "connectionString");

        optionsBuilder.UseSqlServer(connectionString);
    }

    protected virtual void ConfigureStagingContext(string? dbName)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        optionsBuilder.UseSqlServer(config.GetConnectionString(CheckDbName(dbName)));
    }

    protected virtual void ConfigureInMemoryContext(string? dbName)
    {
        optionsBuilder.UseInMemoryDatabase(CheckDbName(dbName));
    }

    private static string CheckDbName(string? dbName) =>
        !string.IsNullOrWhiteSpace(dbName) ? dbName : throw new ArgumentNullException(nameof(dbName));
}
