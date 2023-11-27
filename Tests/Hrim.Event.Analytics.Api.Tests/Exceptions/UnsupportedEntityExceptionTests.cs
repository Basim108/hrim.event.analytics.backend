using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Exceptions;

namespace Hrim.Event.Analytics.Api.Tests.Exceptions;

public class UnsupportedEntityExceptionTests
{
    [Fact]
    public void Should_Set_Message_and_EntityType_Property() {
        var entityType = typeof(string);
        var ex         = new UnsupportedEntityException(entityType: entityType);
        ex.Message.Should().Be($"Unsupported entity of type: {entityType.FullName}");
        ex.EntityType.Should().Be(expected: entityType);
    }
}