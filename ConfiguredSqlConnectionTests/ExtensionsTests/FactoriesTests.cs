using Moq;
using Xunit;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ConfiguredSqlConnection.Extensions;

namespace ConfiguredSqlConnectionTests.ExtensionsTests;

public class FactoriesTests
{
    private readonly ContextOption contextOption;
    private readonly DbContextOptionsBuilder<DbContext> optionsBuilder;
    private readonly DbContextOptionsBuilderFactory<DbContext> optionsBuilderFactory;

    public FactoriesTests()
    {
        contextOption = ContextOption.Prod;
        optionsBuilder = new DbContextOptionsBuilder<DbContext>().UseInMemoryDatabase("dbName");
        var mockOptionsBuilderFactory = new Mock<DbContextOptionsBuilderFactory<DbContext>>();
        mockOptionsBuilderFactory.Setup(x => x.Create(contextOption, null)).Returns(optionsBuilder);
        optionsBuilderFactory = mockOptionsBuilderFactory.Object;
    }

    [Fact]
    public void Create()
    {
        var factory = new Mock<DbContextFactory<DbContext>>(optionsBuilderFactory);
        factory.Setup(x => x.Create(contextOption, null)).CallBase();

        var result = factory.Object.Create(contextOption);

        Assert.NotNull(result);
    }

    [Fact]
    public void CreateFromEnvironment()
    {
        Environment.SetEnvironmentVariable("CONFIGUREDSQLCONNECTION_DB_MODE", $"{contextOption}");
        Environment.SetEnvironmentVariable("CONFIGUREDSQLCONNECTION_DB_NAME", "");
        var factory = new Mock<DbContextEnvironmentFactory<DbContext>>(optionsBuilderFactory);
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
        var factory = new Mock<DbContextEnvironmentFactory<DbContext>>(optionsBuilderFactory);
        factory.Setup(x => x.Create(contextOption, null)).CallBase();
        factory.Setup(x => x.CreateFromEnvironment()).CallBase();

        var exception = Assert.Throws<TargetInvocationException>(() => factory.Object.CreateFromEnvironment());

        Assert.Equal(expectedExceptionMessage, exception?.InnerException?.Message);
    }

    [Fact]
    public void CreateFromEnvironment_InvaledOption_ThrowException()
    {
        var invalidOption = "Production";
        var expectedExceptionMessage = $"Failed to convert environment variable 'CONFIGUREDSQLCONNECTION_DB_MODE' to type '{typeof(ContextOption).FullName}'.";
        Environment.SetEnvironmentVariable("CONFIGUREDSQLCONNECTION_DB_MODE", $"{invalidOption}");
        Environment.SetEnvironmentVariable("CONFIGUREDSQLCONNECTION_DB_NAME", $"");
        var factory = new Mock<DbContextEnvironmentFactory<DbContext>>(optionsBuilderFactory);
        factory.Setup(x => x.Create(contextOption, null)).CallBase();
        factory.Setup(x => x.CreateFromEnvironment()).CallBase();

        var exception = Assert.Throws<TargetInvocationException>(() => factory.Object.CreateFromEnvironment());

        Assert.NotNull(exception.InnerException);
        Assert.Equal(expectedExceptionMessage, exception.InnerException.Message);
    }
}