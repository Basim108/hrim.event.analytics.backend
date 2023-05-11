// Copyright © 2021 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using AutoMapper;
using FluentAssertions;
using Hrim.Event.Analytics.EfCore.AutoMapper.Converters;

namespace Hrim.Event.Analytics.Api.Tests.Converters;

public class NullableDateTimeOffsetConverterTests
{
#pragma warning disable CS8625
    private readonly ResolutionContext _context = null;
#pragma warning restore CS8625
    private readonly NullableDateTimeOffsetConverter _converter = new();

    [Fact]
    public void Should_Convert_From_DateTimeOffset() {
        var now    = DateTimeOffset.Now;
        var result = _converter.Convert(source: now, context: _context);
        result.Should().NotBeNull();
        result!.Value.Year.Should().Be(expected: now.Year);
        result.Value.Month.Should().Be(expected: now.Month);
        result.Value.Day.Should().Be(expected: now.Day);
    }

    [Fact]
    public void Should_Convert_From_Null() {
        var now    = DateTimeOffset.Now;
        var result = _converter.Convert(source: null, context: _context);
        result.Should().BeNull();
    }
}