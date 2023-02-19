using System.Diagnostics.CodeAnalysis;
using Hrim.Event.Analytics.EfCore;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure;

/// <summary>
///     Helpers for test use of DbContext
/// </summary>
[ExcludeFromCodeCoverage]
public static class DbUtils
{
    /// <summary>
    ///     Creates a new DbContext with unique database name
    /// </summary>
    public static EventAnalyticDbContext GetDbContext()
    {
        var builder = new DbContextOptionsBuilder<EventAnalyticDbContext>();
        builder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        var dbContext = new EventAnalyticDbContext(builder.Options);
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
        return dbContext;
    }
}