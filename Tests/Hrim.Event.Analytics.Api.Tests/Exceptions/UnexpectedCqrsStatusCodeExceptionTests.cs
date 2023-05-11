// Copyright © 2021 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Exceptions;

namespace Hrim.Event.Analytics.Api.Tests.Exceptions;

public class UnexpectedCqrsStatusCodeExceptionTests
{
    [Fact]
    public void Should_Set_Message_and_StatusCode_Property() {
        var code = CqrsResultCode.Created;
        var ex   = new UnexpectedCqrsStatusCodeException(statusCode: code);
        ex.Message.Should().Be($"Unexpected CqrsResultCode={code}");
        ex.StatusCode.Should().Be(expected: code);
    }
}