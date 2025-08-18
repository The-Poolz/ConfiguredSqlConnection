using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ConfiguredSqlConnection.Abstractions.Extensions;

namespace ConfiguredSqlConnection.Abstractions.Tests.Extensions;

public class FactoriesTests
{
    private readonly ContextOption contextOption;
    private readonly DbContextOptionsBuilderFactory<DbContext> optionsBuilderFactory;

    public FactoriesTests()
    {
        contextOption = ContextOption.Prod;
        var optionsBuilder = new DbContextOptionsBuilder<DbContext>().UseInMemoryDatabase("dbName");
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

        result.Should().NotBeNull();
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

        result.Should().NotBeNull();
    }

    [Fact]
    public void CreateFromEnvironment_OptionNotSet_ThrowException()
    {
        var expectedExceptionMessage = "Environment variable 'CONFIGUREDSQLCONNECTION_DB_MODE' is null or empty. (Parameter 'envValue')";
        Environment.SetEnvironmentVariable("CONFIGUREDSQLCONNECTION_DB_MODE", $"");
        Environment.SetEnvironmentVariable("CONFIGUREDSQLCONNECTION_DB_NAME", $"");
        var factory = new Mock<DbContextEnvironmentFactory<DbContext>>(optionsBuilderFactory);
        factory.Setup(x => x.Create(contextOption, null)).CallBase();
        factory.Setup(x => x.CreateFromEnvironment()).CallBase();

        var act = factory.Object.CreateFromEnvironment;

        act.Should().Throw<ArgumentNullException>()
            .WithMessage(expectedExceptionMessage);
    }

    [Fact]
    public void CreateFromEnvironment_InvaledOption_ThrowException()
    {
        var invalidOption = "Production";
        var expectedExceptionMessage = $"Failed to convert environment variable 'CONFIGUREDSQLCONNECTION_DB_MODE' to type '{typeof(ContextOption).FullName}'.";
        Environment.SetEnvironmentVariable("CONFIGUREDSQLCONNECTION_DB_MODE", $"{invalidOption}");
        Environment.SetEnvironmentVariable("CONFIGUREDSQLCONNECTION_DB_NAME", "");
        var factory = new Mock<DbContextEnvironmentFactory<DbContext>>(optionsBuilderFactory);
        factory.Setup(x => x.Create(contextOption, null)).CallBase();
        factory.Setup(x => x.CreateFromEnvironment()).CallBase();

        var act = factory.Object.CreateFromEnvironment;

        act.Should().Throw<InvalidCastException>()
            .WithMessage($"*{expectedExceptionMessage}*");
    }
}