using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ConfiguredSqlConnection.Abstractions.Extensions;

namespace ConfiguredSqlConnection.Abstractions.Tests.Extensions;

public class DbContextEnvironmentFactoryTests
{
    private readonly ContextOption _contextOption = ContextOption.Prod;
    private readonly DbContextOptionsBuilderFactory<DbContext> _optionsBuilderFactory;

    public DbContextEnvironmentFactoryTests()
    {
        var optionsBuilder = new DbContextOptionsBuilder<DbContext>().UseInMemoryDatabase("dbName");
        var mockOptionsBuilderFactory = new Mock<DbContextOptionsBuilderFactory<DbContext>>();
        mockOptionsBuilderFactory.Setup(x => x.Create(_contextOption, null)).Returns(optionsBuilder);
        _optionsBuilderFactory = mockOptionsBuilderFactory.Object;
    }

    [Fact]
    public void CreateFromEnvironment_ReturnsContext()
    {
        UseEnvironment(_contextOption.ToString(), string.Empty);
        var factory = new Mock<DbContextEnvironmentFactory<DbContext>>(_optionsBuilderFactory);
        factory.Setup(x => x.Create(_contextOption, null)).CallBase();
        factory.Setup(x => x.CreateFromEnvironment()).CallBase();

        var result = factory.Object.CreateFromEnvironment();

        result.Should().NotBeNull();
    }

    [Fact]
    public void CreateFromEnvironment_OptionNotSet_Throws()
    {
        UseEnvironment(string.Empty, string.Empty);
        var factory = new Mock<DbContextEnvironmentFactory<DbContext>>(_optionsBuilderFactory);
        factory.Setup(x => x.Create(_contextOption, null)).CallBase();
        factory.Setup(x => x.CreateFromEnvironment()).CallBase();

        var act = factory.Object.CreateFromEnvironment;

        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Environment variable 'CONFIGUREDSQLCONNECTION_DB_MODE' is null or empty. (Parameter 'envValue')");
    }

    [Fact]
    public void CreateFromEnvironment_InvalidOption_Throws()
    {
        UseEnvironment("Production", string.Empty);
        var factory = new Mock<DbContextEnvironmentFactory<DbContext>>(_optionsBuilderFactory);
        factory.Setup(x => x.Create(_contextOption, null)).CallBase();
        factory.Setup(x => x.CreateFromEnvironment()).CallBase();

        var act = factory.Object.CreateFromEnvironment;

        act.Should().Throw<InvalidCastException>()
            .WithMessage($"Failed to convert environment variable 'CONFIGUREDSQLCONNECTION_DB_MODE' to type '{typeof(ContextOption).FullName}'.");
    }

    private static void UseEnvironment(string? mode, string? name)
    {
        Environment.SetEnvironmentVariable("CONFIGUREDSQLCONNECTION_DB_MODE", mode);
        Environment.SetEnvironmentVariable("CONFIGUREDSQLCONNECTION_DB_NAME", name);
    }
}