using Microsoft.EntityFrameworkCore;

namespace ConfiguredSqlConnection.PostgresSql.Tests;

internal class TestDbContext(DbContextOptions options) : DbContext(options);