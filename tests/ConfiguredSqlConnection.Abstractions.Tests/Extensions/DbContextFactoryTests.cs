using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ConfiguredSqlConnection.Abstractions.Extensions;

namespace ConfiguredSqlConnection.Abstractions.Tests.Extensions;

public class DbContextFactoryTests
{
    private readonly ContextOption _contextOption = ContextOption.Prod;
    private readonly DbContextOptionsBuilderFactory<DbContext> _optionsBuilderFactory;

    public DbContextFactoryTests()
    {
        var optionsBuilder = new DbContextOptionsBuilder<DbContext>().UseInMemoryDatabase("dbName");
        var mockOptionsBuilderFactory = new Mock<DbContextOptionsBuilderFactory<DbContext>>();
        mockOptionsBuilderFactory.Setup(x => x.Create(_contextOption, null)).Returns(optionsBuilder);
        _optionsBuilderFactory = mockOptionsBuilderFactory.Object;
    }

    [Fact]
    public void Create_ReturnsDbContext()
    {
        var factory = new Mock<DbContextFactory<DbContext>>(_optionsBuilderFactory);
        factory.Setup(x => x.Create(_contextOption, null)).CallBase();

        var result = factory.Object.Create(_contextOption);

        result.Should().NotBeNull();
    }
}