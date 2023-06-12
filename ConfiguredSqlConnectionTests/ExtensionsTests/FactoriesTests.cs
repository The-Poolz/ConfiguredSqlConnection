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

        optionsBuilder = new DbContextOptionsBuilder<DataBaseContext>()
            .UseInMemoryDatabase("dbName");

        var mockOptionsBuilderFactory = new Mock<DbContextOptionsBuilderFactory<DataBaseContext>>();
        mockOptionsBuilderFactory.Setup(x => x.Create(contextOption, null))
            .Returns(optionsBuilder);

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
        Environment.SetEnvironmentVariable("DB_MODE", $"{contextOption}");
        Environment.SetEnvironmentVariable("DB_NAME", "");
        var factory = new Mock<DbContextEnvironmentFactory>(optionsBuilderFactory);
        factory.Setup(x => x.Create(contextOption, null)).CallBase();
        factory.Setup(x => x.CreateFromEnvironment()).CallBase();

        var result = factory.Object.CreateFromEnvironment();

        Assert.NotNull(result);
    }

    [Fact]
    public void CreateFromEnvironment_OptionNotSet_ThrowException()
    {
        Environment.SetEnvironmentVariable("DB_MODE", $"");
        Environment.SetEnvironmentVariable("DB_NAME", $"");
        var factory = new Mock<DbContextEnvironmentFactory>(optionsBuilderFactory);
        factory.Setup(x => x.Create(contextOption, null)).CallBase();
        factory.Setup(x => x.CreateFromEnvironment()).CallBase();

        Action testCode = () => factory.Object.CreateFromEnvironment();

        var exception = Assert.Throws<TargetInvocationException>(testCode);
        var expectedExceptionMessage = $"Environment variable 'DB_MODE' is null or empty.";
        Assert.Equal(expectedExceptionMessage, exception?.InnerException?.Message);
    }

    [Fact]
    public void CreateFromEnvironment_InvaledOption_ThrowException()
    {
        var invalidOption = "Production";
        Environment.SetEnvironmentVariable("DB_MODE", $"{invalidOption}");
        Environment.SetEnvironmentVariable("DB_NAME", $"");
        var factory = new Mock<DbContextEnvironmentFactory>(optionsBuilderFactory);
        factory.Setup(x => x.Create(contextOption, null)).CallBase();
        factory.Setup(x => x.CreateFromEnvironment()).CallBase();

        Action testCode = () => factory.Object.CreateFromEnvironment();

        var exception = Assert.Throws<ArgumentException>(testCode);
        var expectedExceptionMessage = $"Invalid value for environment variable 'DB_MODE': {invalidOption}";
        Assert.Equal(expectedExceptionMessage, exception.Message);
    }

}