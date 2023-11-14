# ConfiguredSqlConnection

The NuGet package is a collection of utilities for working with SQL Server database connections using environment settings and secure connection strings. It includes classes such as `EnvManager`, `ConnectionOptionsBuilder`, and `SecretManager` that facilitate retrieving and configuring connection parameters based on environment variables and secure secrets. This package aims to simplify the process of configuring and utilizing SQL Server connections in your application.

## Creating a TContext using DbContextFactory&lt;TContext&gt;

Create a `TContext` instance based on the desired context option:

```csharp
var dbContext = new DbContextFactory<TContext>().Create(ContextOption.Prod, dbName);
```

The following context options are available:
```csharp
public enum ContextOption
{
    Prod,
    Staging,
    InMemory
}
```

- `ContextOption.Prod:` Retrieves the connection string from the environment variable "CONFIGUREDSQLCONNECTION_SECRET_NAME_OF_CONNECTION". This option is suitable for production environments.

- `ContextOption.Staging:` Reads the connection string from the "appsettings.json" file. The `dbName` parameter can be provided to specify the target database name. This option is useful for staging or development environments.

- `ContextOption.InMemory:` Uses an in-memory database for testing or development purposes. The `dbName` parameter can be provided to set a custom database name.

- `dbName` parameter is optional and should be provided only for the *"Staging"* or *"InMemory"* options. It is not required for the *"Prod"* option.

## Creating a TContext using DbContextEnvironmentFactory&lt;TContext&gt;

If you want to switch modes using environment variables, you can use the `DbContextEnvironmentFactory<TContext>` class.
It is derived from the `DbContextFactory<TContext>` and provides additional functionality to retrieve the mode and database name from environment variables.

```csharp
var dbContext = new DbContextEnvironmentFactory().CreateFromEnvironment();
```
The `CreateFromEnvironment()` method retrieves the mode and database name from the environment variables `CONFIGUREDSQLCONNECTION_DB_MODE` and `CONFIGUREDSQLCONNECTION_DB_NAME`, respectively. 

### Environment variables
- `CONFIGUREDSQLCONNECTION_DB_MODE`: environment variable should contain one of the following values: *"Prod"*, *"Staging"*, or *"InMemory"*, which determine the desired context option.
- `CONFIGUREDSQLCONNECTION_DB_NAME`: environment variable can be used to specify the database name for the *"Staging"* or *"InMemory"* options.
- `CONFIGUREDSQLCONNECTION_SECRET_NAME_OF_CONNECTION`:  environment variable should contain the connection string for the *"Prod"* option.
- `CONFIGUREDSQLCONNECTION_ACTION_CONNECTION`: environment variable should contain the connection string. This is required for GitHub Actions to obtain the database connection string, enabling it to execute the migrations. Ensure that your connection string is correctly formatted and points to the appropriate database, as all migrations will be applied there.

### "appsettings.json" file
Ensure that your `appsettings.json` file is located in the root folder of your project. This file should contain the necessary configuration settings, including the connection strings for different contexts.

```json
{
  "ConnectionStrings": {
    "LocalTestDb": "Server=(localdb)\\MyTestDb;Initial Catalog=TestDb",
    "ProdDb": "Data Source=maindatabase.com,1234;User id=testUser;Password=password;Initial Catalog=ProdDb;TrustServerCertificate=true;",
    "StagingDb": "Data Source=maindatabase.com,1234;User id=testUser;Password=password;Initial Catalog=StagingDb;TrustServerCertificate=true;"
  }
}
```

## Creating a customized DbContextOptionsBuilder&lt;TContext&gt;

This class is a factory for creating `DbContextOptionsBuilder<TContext>` instances based on the provided `ContextOption` and optional database name (`dbName`).

```csharp
var optionsBuilder = new DbContextOptionsBuilderFactory<TContext>().Create(ContextOption.YourOption, dbName)
```

## ConnectionStringFactory

This static class provides methods to retrieve connection strings based on the provided `ContextOption` and optional `dbName`.

```csharp
var connectionString = ConnectionStringFactory.GetConnection(ContextOption.YourOption, dbName)
```

## ConnectionStringFromEnvironmentFactory

This static class provides a method to retrieve a connection string from environment variables (`CONFIGUREDSQLCONNECTION_DB_MODE` and `CONFIGUREDSQLCONNECTION_DB_NAME`).

```csharp
var connectionString = ConnectionStringFactory.GetConnectionFromEnvironment()
```

## DbContextOptionsBuilderExtensions

This static class provides an extensions methods to configure a `DbContextOptionsBuilder` instance in method `OnConfiguring` from `DbContext`.
Each these method do nothing if `DbContextOptionsBuilder` already be configured.

```csharp
// Try to configure action connection.
optionsBuilder
    .ConfigureFromActionConnection();

// Try to configure db connection via SecretManager.
optionsBuilder
    .ConfigureFromSecretConnection();

// Try to configure action connection also try to configure db connection via SecretManager.
optionsBuilder
    .ConfigureFromActionConnection()
    .ConfigureFromSecretConnection();

// Try to configure action connection also try to configure db connection via SecretManager.
optionsBuilder
    .ConfigureFromActionConnection()
    .ConfigureFromSecretConnection();
```
