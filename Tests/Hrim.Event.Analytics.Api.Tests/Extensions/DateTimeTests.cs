using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Extensions;

namespace Hrim.Event.Analytics.Api.Tests.Extensions;

public class DateTimeTests
{
    [Fact]
    public void DateTime_To_DateOnly_Should_Convert_Correctly() {
        var dateOnly = DateTime.UtcNow.ToDateOnly();
        dateOnly.Year.Should().Be(DateTime.UtcNow.Year);
        dateOnly.Month.Should().Be(DateTime.UtcNow.Month);
        dateOnly.Day.Should().Be(DateTime.UtcNow.Day);
    }

    [Fact]
    public void DateTimeOffset_To_DateOnly_Should_Convert_Correctly() {
        var dateOnly = DateTimeOffset.UtcNow.ToDateOnly();
        dateOnly.Year.Should().Be(DateTimeOffset.UtcNow.Year);
        dateOnly.Month.Should().Be(DateTimeOffset.UtcNow.Month);
        dateOnly.Day.Should().Be(DateTimeOffset.UtcNow.Day);
    }
}