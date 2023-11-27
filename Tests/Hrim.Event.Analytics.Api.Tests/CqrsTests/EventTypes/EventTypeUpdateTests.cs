using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Api.Tests.Infrastructure.AssertHelpers;
using Hrim.Event.Analytics.Api.V1.Models;
using EventType = Hrim.Event.Analytics.Abstractions.Entities.EventTypes.EventType;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.EventTypes;

[ExcludeFromCodeCoverage]
public class EventTypeCqrsTests: BaseCqrsTests
{
    /// <summary> Correct update event type request  </summary>
    private readonly UpdateEventTypeRequest _updateEventTypeRequest = new() {
        Id              = new Random().NextInt64(),
        ConcurrentToken = 1,
        Name            = "Headache",
        Color           = "#ff0000",
        Description     = "times when I had a headache",
        IsPublic        = true
    };

    [Fact]
    public async Task Update_EventType() {
        var createdEntities = TestData.Events.CreateManyEventTypes(count: 1, userId: OperatorUserId);
        var forUpdate       = new EventType();
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
        var cqrsResult = await Mediator.Send(new EventTypeUpdateCommand(new EventType {
                                                                                    Id    = new Random().NextInt64(),
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
        var forUpdate       = new EventType();
        createdEntities.First().Value.CopyTo(another: forUpdate);
        forUpdate.ConcurrentToken = 3;
        var updateCommand = new EventTypeUpdateCommand(EventType: forUpdate, SaveChanges: true, Context: OperatorContext);

        var cqrsResult = await Mediator.Send(request: updateCommand);

        cqrsResult.CheckConcurrentConflictUpdate(forUpdate: forUpdate);
    }

    [Fact]
    public async Task Update_Already_Deleted_Entity() {
        var createdEntities = TestData.Events.CreateManyEventTypes(count: 1, userId: OperatorUserId, isDeleted: true);
        var forUpdate       = new EventType();
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
        var anotherUserId = new Random().NextInt64();
        TestData.Users.EnsureUserExistence(id: anotherUserId);
        var createdEntities = TestData.Events.CreateManyEventTypes(count: 1, userId: anotherUserId);
        var forUpdate       = new EventType();
        createdEntities.First().Value.CopyTo(another: forUpdate);
        var updateCommand = new EventTypeUpdateCommand(EventType: forUpdate, SaveChanges: true, Context: OperatorContext);

        var cqrsResult = await Mediator.Send(request: updateCommand);

        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.Forbidden);
    }

    [Fact]
    public async Task Update_Given_EventType_With_AnalysisResults_Should_Not_Save_Them_To_DB() {
        var createdEntities = TestData.Events.CreateManyEventTypes(count: 1, userId: OperatorUserId);
        var forUpdate       = new EventType();
        createdEntities.First().Value.CopyTo(another: forUpdate);
        forUpdate.Name = "Updated";
        var beforeSend    = DateTime.UtcNow;
        var updateCommand = new EventTypeUpdateCommand(EventType: forUpdate, SaveChanges: true, Context: OperatorContext);

        updateCommand.EventType.AnalysisResults = new List<StatisticsForEventType> {
            new() {
                EntityId     = updateCommand.EventType.Id,
                AnalysisCode = FeatureCodes.GAP_ANALYSIS,
                ResultJson   = "",
                StartedAt    = DateTime.UtcNow,
                FinishedAt   = DateTime.UtcNow.AddMinutes(1)
            }
        };

        var cqrsResult = await Mediator.Send(request: updateCommand);

        cqrsResult.CheckSuccessfullyUpdatedEntity(operatorId: OperatorUserId, forUpdate: forUpdate, beforeSend: beforeSend);

        TestData.DbContext.StatisticsForEventTypes.ToList().Should().BeEmpty();
    }
    
    [Fact]
    public async Task Given_EventType_With_Parent_Should_Update_TreeNodePath() {
        var parent    = TestData.Events.CreateEventType(OperatorUserId, "Parent Test Event Type");
        var eventType = TestData.Events.CreateEventType(OperatorUserId);
        eventType.Bl.Parent   = parent.Bl;
        eventType.Bl.ParentId = parent.Bl.Id;

        var updateCommand = new EventTypeUpdateCommand(eventType.Bl, SaveChanges: true, Context: OperatorContext);
        await Mediator.Send(request: updateCommand);

        var updatedEventType = TestData.DbContext.EventTypes.FirstOrDefault(x => x.Id == eventType.Bl.Id);
        updatedEventType.Should().NotBeNull();
        updatedEventType!.TreeNodePath.ToString().Should().Be($"{parent.Bl.Id}.{eventType.Bl.Id}");
    }
    
    [Fact]
    public async Task Given_Not_Existing_ParentId_Should_Not_Change_Parent() {
        var parent    = TestData.Events.CreateEventType(OperatorUserId, "Parent Test Event Type");
        var eventType = TestData.Events.CreateEventType(OperatorUserId, parentId: parent.Db.Id, updateTreeNode: true);
        eventType.Bl.ParentId = new Random().NextInt64();

        var updateCommand = new EventTypeUpdateCommand(eventType.Bl, SaveChanges: true, Context: OperatorContext);
        await Mediator.Send(request: updateCommand);

        var updatedEventType = TestData.DbContext.EventTypes.FirstOrDefault(x => x.Id == eventType.Bl.Id);
        updatedEventType.Should().NotBeNull();
        updatedEventType!.ParentId.Should().Be(parent.Db.Id);
        updatedEventType.TreeNodePath.ToString().Should().Be($"{parent.Db.Id}.{updatedEventType.Id}");
    }
    
    [Fact]
    public async Task Given_Null_ParentId_Should_Set_Parent_To_Null() {
        var parent    = TestData.Events.CreateEventType(OperatorUserId, "Parent Test Event Type");
        var eventType = TestData.Events.CreateEventType(OperatorUserId, parentId: parent.Db.Id, updateTreeNode: true);
        eventType.Bl.ParentId = null;

        var updateCommand = new EventTypeUpdateCommand(eventType.Bl, SaveChanges: true, Context: OperatorContext);
        await Mediator.Send(request: updateCommand);

        var updatedEventType = TestData.DbContext.EventTypes.FirstOrDefault(x => x.Id == eventType.Bl.Id);
        updatedEventType.Should().NotBeNull();
        updatedEventType!.ParentId.Should().BeNull();
        updatedEventType.TreeNodePath.ToString().Should().Be(updatedEventType.Id.ToString());
    }
}