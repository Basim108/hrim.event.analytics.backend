using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.EventTypes;

[ExcludeFromCodeCoverage]
public class EventTypeGetByIdTests: BaseCqrsTests
{
    [Fact]
    public async Task GetById_Returns_Owned_EventTypes() {
        var anotherUserId = Guid.NewGuid();
        TestData.Users.EnsureUserExistence(id: anotherUserId);
        TestData.Events.CreateManyEventTypes(count: 1, userId: anotherUserId);
        var myEventIds = TestData.Events.CreateManyEventTypes(count: 4, userId: OperatorUserId);

        var targetEntity = myEventIds.First().Value;
        var cqrsResult   = await Mediator.Send(new EventTypeGetById(Id: targetEntity.Id, IsNotTrackable: true, Context: OperatorContext));
        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.Ok);
        cqrsResult.Result.Should().NotBeNull();
        cqrsResult.Result!.Id.Should().Be(expected: targetEntity.Id);
    }

    [Fact]
    public async Task GetById_Given_AnotherId_Returns_Forbidden() {
        var anotherUserId = Guid.NewGuid();
        TestData.Users.EnsureUserExistence(id: anotherUserId);
        var anotherEntityId = TestData.Events.CreateManyEventTypes(count: 1, userId: anotherUserId).Keys.First();
        TestData.Events.CreateManyEventTypes(count: 4, userId: OperatorUserId);

        var cqrsResult = await Mediator.Send(new EventTypeGetById(Id: anotherEntityId, IsNotTrackable: true, Context: OperatorContext));
        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.Forbidden);
    }
}