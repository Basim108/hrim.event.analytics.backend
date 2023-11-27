using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Exceptions;

namespace Hrim.Event.Analytics.Api.Tests.Exceptions;

public class UnsupportedAuthTypeExceptionTests
{
    [Fact]
    public void Should_Set_Message_and_AuthenticationType_Property() {
        var authType = "Bearer";
        var ex       = new UnsupportedAuthTypeException(authType: authType);
        ex.Message.Should().Be($"Unsupported authentication type: {authType}");
        ex.AuthenticationType.Should().Be(expected: authType);
    }

    [Theory]
    [InlineData(data: null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_Replace_Empty_String_With_Meaningful_Value(string? authType) {
        var ex = new UnsupportedAuthTypeException(authType: authType);
        ex.Message.Should().Be(expected: "Unsupported authentication type: null or white space");
        ex.AuthenticationType.Should().Be(expected: "null or white space");
    }
}