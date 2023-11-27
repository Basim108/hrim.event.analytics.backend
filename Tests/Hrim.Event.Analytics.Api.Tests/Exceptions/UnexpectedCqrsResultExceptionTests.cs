using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Exceptions;

namespace Hrim.Event.Analytics.Api.Tests.Exceptions;

public class UnexpectedCqrsResultExceptionTests
{
    [Fact]
    public void Should_Set_Message_and_CqrsResult_Property() {
        var cqrsResult = new CqrsResult<DurationEvent?>(Result: null, StatusCode: CqrsResultCode.Conflict);
        var ex         = new UnexpectedCqrsResultException<DurationEvent?>(cqrsResult: cqrsResult);
        ex.Message.StartsWith(value: "Unexpected CqrsResult").Should().BeTrue();
        ex.Message.Contains(CqrsResultCode.Conflict.ToString()).Should().BeTrue();
        ex.CqrsResult.Should().Be(expected: cqrsResult);
    }
}