using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;
using Hrim.Event.Analytics.Api.V1.Models;

namespace Hrim.Event.Analytics.Api.Tests.Cqrs.EventTypes;

[ExcludeFromCodeCoverage]
public class EventTypeCqrsTests: BaseCqrsTests {
    /// <summary> Correct update event type request  </summary>
    private readonly UpdateEventTypeRequest _updateEventTypeRequest = new() {
        Id              = Guid.NewGuid(),
        ConcurrentToken = 1,
        Name            = "Headache",
        Color           = "#ff0000",
        Description     = "times when I had a headache",
        IsPublic        = true
    };

    [Fact]
    public async Task Update_EventType() {
        var createdEntities = TestData.CreateManyEventTypes(1, OperatorContext.UserId);
        var forUpdate       = new UserEventType();
        createdEntities.First().Value.CopyTo(forUpdate);
        forUpdate.Name = "Updated";
        var beforeSend    = DateTime.UtcNow;
        var updateCommand = new EventTypeUpdateCommand(forUpdate, SaveChanges: true, OperatorContext);

        var cqrsResult = await Mediator.Send(updateCommand);

        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(CqrsResultCode.Ok);
        var resultEventType = cqrsResult.Result;
        resultEventType.Should().NotBeNull();
        resultEventType!.Id.Should().Be(forUpdate.Id);
        resultEventType.CreatedById.Should().NotBeEmpty();
        resultEventType.CreatedAt.Should().Be(forUpdate.CreatedAt);
        resultEventType.UpdatedAt.Should().BeAfter(beforeSend);
        resultEventType.IsDeleted.Should().BeNull();
        resultEventType.ConcurrentToken.Should().Be(2);

        resultEventType.Name.Should().Be("Updated");
        resultEventType.Color.Should().Be(forUpdate.Color);
        resultEventType.Description.Should().Be(forUpdate.Description);
        resultEventType.IsPublic.Should().Be(forUpdate.IsPublic);
    }

    [Fact]
    public async Task Update_NotFound_EventType() {
        var cqrsResult = await Mediator.Send(new EventTypeUpdateCommand(new UserEventType() {
            Id    = Guid.NewGuid(),
            Color = _updateEventTypeRequest.Color,
            Name  = _updateEventTypeRequest.Name
        }, SaveChanges: true, OperatorContext));
        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(CqrsResultCode.NotFound);
    }

    [Fact]
    public async Task Update_With_Wrong_ConcurrentToken() {
        var createdEntities = TestData.CreateManyEventTypes(1, OperatorContext.UserId);
        var forUpdate       = new UserEventType();
        createdEntities.First().Value.CopyTo(forUpdate);
        forUpdate.ConcurrentToken = 3;
        var updateCommand = new EventTypeUpdateCommand(forUpdate, SaveChanges: true, OperatorContext);

        var cqrsResult = await Mediator.Send(updateCommand);

        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(CqrsResultCode.Conflict);
        cqrsResult.Result.Should().NotBeNull();
        cqrsResult.Result!.Id.Should().Be(forUpdate.Id);
        cqrsResult.Result.ConcurrentToken.Should().Be(1);
        cqrsResult.Result.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public async Task Update_Already_Deleted_Entity() {
        var createdEntities = TestData.CreateManyEventTypes(1, OperatorContext.UserId, isDeleted: true);
        var forUpdate       = new UserEventType();
        createdEntities.First().Value.CopyTo(forUpdate);
        var updateCommand = new EventTypeUpdateCommand(forUpdate, SaveChanges: true, OperatorContext);

        var cqrsResult = await Mediator.Send(updateCommand);

        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(CqrsResultCode.EntityIsDeleted);
        cqrsResult.Result.Should().NotBeNull();
        cqrsResult.Result!.Id.Should().Be(forUpdate.Id);
        cqrsResult.Result.ConcurrentToken.Should().Be(1);
        cqrsResult.Result.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task Update_Given_Not_My_EventType_Forbid() {
        var anotherUserId = Guid.NewGuid();
        TestData.CreateUser(anotherUserId);
        var createdEntities = TestData.CreateManyEventTypes(1, anotherUserId);
        var forUpdate       = new UserEventType();
        createdEntities.First().Value.CopyTo(forUpdate);
        var updateCommand = new EventTypeUpdateCommand(forUpdate, SaveChanges: true, OperatorContext);

        var cqrsResult = await Mediator.Send(updateCommand);

        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(CqrsResultCode.Forbidden);
    }
}