using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.EventTypes;

[ExcludeFromCodeCoverage]
public class EventTypeGetByIdTests : BaseCqrsTests
{
    [Fact]
    public async Task GetById_Returns_Owned_EventTypes()
    {
        var anotherUserId = Guid.NewGuid();
        TestData.Users.EnsureUserExistence(anotherUserId);
        TestData.Events.CreateManyEventTypes(1, anotherUserId);
        var myEventIds = TestData.Events.CreateManyEventTypes(4, OperatorContext.UserId);

        var targetEntity = myEventIds.First().Value;
        var cqrsResult = await Mediator.Send(new EventTypeGetById(targetEntity.Id, true, OperatorContext));
        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(CqrsResultCode.Ok);
        cqrsResult.Result.Should().NotBeNull();
        cqrsResult.Result!.Id.Should().Be(targetEntity.Id);
    }

    [Fact]
    public async Task GetById_Given_AnotherId_Returns_Forbidden()
    {
        var anotherUserId = Guid.NewGuid();
        TestData.Users.EnsureUserExistence(anotherUserId);
        var anotherEntityId = TestData.Events.CreateManyEventTypes(1, anotherUserId).Keys.First();
        TestData.Events.CreateManyEventTypes(4, OperatorContext.UserId);

        var cqrsResult = await Mediator.Send(new EventTypeGetById(anotherEntityId, true, OperatorContext));
        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(CqrsResultCode.Forbidden);
    }
}