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