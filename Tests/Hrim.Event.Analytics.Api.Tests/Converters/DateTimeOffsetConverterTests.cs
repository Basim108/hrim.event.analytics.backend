using AutoMapper;
using FluentAssertions;
using Hrim.Event.Analytics.EfCore.AutoMapper.Converters;

namespace Hrim.Event.Analytics.Api.Tests.Converters;

public class DateTimeOffsetConverterTests
{
#pragma warning disable CS8625
    private readonly ResolutionContext _context = null;
#pragma warning restore CS8625
    private readonly DateTimeOffsetConverter _converter = new();

    [Fact]
    public void Should_Convert_To_DateOnly() {
        var now    = DateTimeOffset.Now;
        var result = _converter.Convert(source: now, context: _context);
        result.Year.Should().Be(expected: now.Year);
        result.Month.Should().Be(expected: now.Month);
        result.Day.Should().Be(expected: now.Day);
    }
}