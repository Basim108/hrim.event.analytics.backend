// Copyright © 2021 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using FluentAssertions;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Exceptions;

namespace Hrim.Event.Analytics.Api.Tests.Exceptions;

public class UnsupportedAuthTypeExceptionTests
{
    [Fact]
    public void Should_Set_Message_and_AuthenticationType_Property() {
        var authType = "Bearer";
        var ex       = new UnsupportedAuthTypeException(authType);
        ex.Message.Should().Be($"Unsupported authentication type: {authType}");
        ex.AuthenticationType.Should().Be(authType);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_Replace_Empty_String_With_Meaningful_Value(string? authType) {
        var ex = new UnsupportedAuthTypeException(authType);
        ex.Message.Should().Be("Unsupported authentication type: null or white space");
        ex.AuthenticationType.Should().Be("null or white space");
    }
}