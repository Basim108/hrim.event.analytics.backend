using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;

namespace Hrim.Event.Analytics.Api.Tests.Cqrs.EventTypes;

[ExcludeFromCodeCoverage]
public class EventTypeGetByIdTests: BaseCqrsTests {
    [Fact]
    public async Task GetById_Returns_Owned_EventTypes() {
        var anotherUserId = Guid.NewGuid();
        TestData.CreateUser(anotherUserId);
        TestData.CreateManyEventTypes(1, anotherUserId);
        var myEventIds = TestData.CreateManyEventTypes(4, OperatorContext.UserId);

        var targetEntity = myEventIds.First().Value;
        var cqrsResult   = await Mediator.Send(new EventTypeGetById(targetEntity.Id, IsNotTrackable: true, OperatorContext));
        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(CqrsResultCode.Ok);
        cqrsResult.Result.Should().NotBeNull();
        cqrsResult.Result!.Id.Should().Be(targetEntity.Id);
    }

    [Fact]
    public async Task GetById_Given_AnotherId_Returns_Forbidden() {
        var anotherUserId = Guid.NewGuid();
        TestData.CreateUser(anotherUserId);
        var anotherEntityId = TestData.CreateManyEventTypes(1, anotherUserId).Keys.First();
        TestData.CreateManyEventTypes(4, OperatorContext.UserId);

        var cqrsResult = await Mediator.Send(new EventTypeGetById(anotherEntityId, IsNotTrackable: true, OperatorContext));
        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(CqrsResultCode.Forbidden);
    }
}