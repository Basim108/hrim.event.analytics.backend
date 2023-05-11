using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Api.Tests.Infrastructure.AssertHelpers;
using Hrim.Event.Analytics.Api.V1.Models;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.EventTypes;

[ExcludeFromCodeCoverage]
public class EventTypeCqrsTests: BaseCqrsTests
{
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
        var createdEntities = TestData.Events.CreateManyEventTypes(count: 1, userId: OperatorUserId);
        var forUpdate       = new UserEventType();
        createdEntities.First().Value.CopyTo(another: forUpdate);
        forUpdate.Name = "Updated";
        var beforeSend    = DateTime.UtcNow;
        var updateCommand = new EventTypeUpdateCommand(EventType: forUpdate, SaveChanges: true, Context: OperatorContext);

        var cqrsResult = await Mediator.Send(request: updateCommand);

        cqrsResult.CheckSuccessfullyUpdatedEntity(operatorId: OperatorUserId, forUpdate: forUpdate, beforeSend: beforeSend);

        cqrsResult.Result!.Name.Should().Be(expected: "Updated");
        cqrsResult.Result.Color.Should().Be(expected: forUpdate.Color);
        cqrsResult.Result.Description.Should().Be(expected: forUpdate.Description);
        cqrsResult.Result.IsPublic.Should().Be(expected: forUpdate.IsPublic);
    }

    [Fact]
    public async Task Update_NotFound_EventType() {
        var cqrsResult = await Mediator.Send(new EventTypeUpdateCommand(new UserEventType {
                                                                            Id    = Guid.NewGuid(),
                                                                            Color = _updateEventTypeRequest.Color,
                                                                            Name  = _updateEventTypeRequest.Name
                                                                        },
                                                                        SaveChanges: true,
                                                                        Context: OperatorContext));
        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.NotFound);
    }

    [Fact]
    public async Task Update_With_Wrong_ConcurrentToken() {
        var createdEntities = TestData.Events.CreateManyEventTypes(count: 1, userId: OperatorUserId);
        var forUpdate       = new UserEventType();
        createdEntities.First().Value.CopyTo(another: forUpdate);
        forUpdate.ConcurrentToken = 3;
        var updateCommand = new EventTypeUpdateCommand(EventType: forUpdate, SaveChanges: true, Context: OperatorContext);

        var cqrsResult = await Mediator.Send(request: updateCommand);

        cqrsResult.CheckConcurrentConflictUpdate(forUpdate: forUpdate);
    }

    [Fact]
    public async Task Update_Already_Deleted_Entity() {
        var createdEntities = TestData.Events.CreateManyEventTypes(count: 1, userId: OperatorUserId, isDeleted: true);
        var forUpdate       = new UserEventType();
        createdEntities.First().Value.CopyTo(another: forUpdate);
        var updateCommand = new EventTypeUpdateCommand(EventType: forUpdate, SaveChanges: true, Context: OperatorContext);

        var cqrsResult = await Mediator.Send(request: updateCommand);

        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.EntityIsDeleted);
        cqrsResult.Result.Should().NotBeNull();
        cqrsResult.Result!.Id.Should().Be(expected: forUpdate.Id);
        cqrsResult.Result.ConcurrentToken.Should().Be(expected: 1);
        cqrsResult.Result.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task Update_Given_Not_My_EventType_Forbid() {
        var anotherUserId = Guid.NewGuid();
        TestData.Users.EnsureUserExistence(id: anotherUserId);
        var createdEntities = TestData.Events.CreateManyEventTypes(count: 1, userId: anotherUserId);
        var forUpdate       = new UserEventType();
        createdEntities.First().Value.CopyTo(another: forUpdate);
        var updateCommand = new EventTypeUpdateCommand(EventType: forUpdate, SaveChanges: true, Context: OperatorContext);

        var cqrsResult = await Mediator.Send(request: updateCommand);

        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.Forbidden);
    }
}