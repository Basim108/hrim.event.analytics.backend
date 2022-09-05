using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Api.Tests.Infrastructure.AssertHelpers;
using Hrim.Event.Analytics.Api.V1.Models;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.EventTypes;

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

        cqrsResult.CheckSuccessfullyUpdatedEntity(OperatorContext.UserId, forUpdate, beforeSend);
        
        cqrsResult.Result!.Name.Should().Be("Updated");
        cqrsResult.Result.Color.Should().Be(forUpdate.Color);
        cqrsResult.Result.Description.Should().Be(forUpdate.Description);
        cqrsResult.Result.IsPublic.Should().Be(forUpdate.IsPublic);
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

        cqrsResult.CheckConcurrentConflictUpdate(forUpdate);
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