using ConfiguredSqlConnection;
using ConfiguredSqlConnection.Extensions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Reflection;
using Xunit;

namespace ConfiguredSqlConnectionTests.ExtensionsTests;

public class FactoriesTests
{
    private readonly ContextOption contextOption;
    private readonly DbContextOptionsBuilder<DataBaseContext> optionsBuilder;
    private readonly DbContextOptionsBuilderFactory<DataBaseContext> optionsBuilderFactory;

    public FactoriesTests()
    {
        contextOption = ContextOption.Prod;
        optionsBuilder = new DbContextOptionsBuilder<DataBaseContext>().UseInMemoryDatabase("dbName");
        var mockOptionsBuilderFactory = new Mock<DbContextOptionsBuilderFactory<DataBaseContext>>();
        mockOptionsBuilderFactory.Setup(x => x.Create(contextOption, null)).Returns(optionsBuilder);
        optionsBuilderFactory = mockOptionsBuilderFactory.Object;
    }

    [Fact]
    public void Create()
    {
        var factory = new Mock<DbContextFactory>(optionsBuilderFactory);
        factory.Setup(x => x.Create(contextOption, null)).CallBase();

        var result = factory.Object.Create(contextOption);

        Assert.NotNull(result);
    }

    [Fact]
    public void CreateFromEnvironment()
    {
        Environment.SetEnvironmentVariable("CONFIGUREDSQLCONNECTION_DB_MODE", $"{contextOption}");
        Environment.SetEnvironmentVariable("CONFIGUREDSQLCONNECTION_DB_NAME", "");
        var factory = new Mock<DbContextEnvironmentFactory>(optionsBuilderFactory);
        factory.Setup(x => x.Create(contextOption, null)).CallBase();
        factory.Setup(x => x.CreateFromEnvironment()).CallBase();

        var result = factory.Object.CreateFromEnvironment();

        Assert.NotNull(result);
    }

    [Fact]
    public void CreateFromEnvironment_OptionNotSet_ThrowException()
    {
        var expectedExceptionMessage = $"Environment variable 'CONFIGUREDSQLCONNECTION_DB_MODE' is null or empty.";
        Environment.SetEnvironmentVariable("CONFIGUREDSQLCONNECTION_DB_MODE", $"");
        Environment.SetEnvironmentVariable("CONFIGUREDSQLCONNECTION_DB_NAME", $"");
        var factory = new Mock<DbContextEnvironmentFactory>(optionsBuilderFactory);
        factory.Setup(x => x.Create(contextOption, null)).CallBase();
        factory.Setup(x => x.CreateFromEnvironment()).CallBase();

        var exception = Assert.Throws<TargetInvocationException>(() => factory.Object.CreateFromEnvironment());

        Assert.Equal(expectedExceptionMessage, exception?.InnerException?.Message);
    }

    [Fact]
    public void CreateFromEnvironment_InvaledOption_ThrowException()
    {
        var invalidOption = "Production";
        var expectedExceptionMessage = $"Invalid value for environment variable 'CONFIGUREDSQLCONNECTION_DB_MODE': {invalidOption}";
        Environment.SetEnvironmentVariable("CONFIGUREDSQLCONNECTION_DB_MODE", $"{invalidOption}");
        Environment.SetEnvironmentVariable("CONFIGUREDSQLCONNECTION_DB_NAME", $"");
        var factory = new Mock<DbContextEnvironmentFactory>(optionsBuilderFactory);
        factory.Setup(x => x.Create(contextOption, null)).CallBase();
        factory.Setup(x => x.CreateFromEnvironment()).CallBase();

        var exception = Assert.Throws<ArgumentException>(() => factory.Object.CreateFromEnvironment());
  
        Assert.Equal(expectedExceptionMessage, exception.Message);
    }
}