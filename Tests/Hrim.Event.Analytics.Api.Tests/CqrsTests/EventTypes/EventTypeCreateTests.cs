using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.Api.Tests.Infrastructure.AssertHelpers;
using Hrim.Event.Analytics.Api.V1.Models;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.EventTypes;

[ExcludeFromCodeCoverage]
public class EventTypeCreateTests: BaseCqrsTests
{
    private readonly EventTypeCreateCommand _createCommand;

    /// <summary> Correct create event type request  </summary>
    private readonly CreateEventTypeRequest _createEventTypeRequest = new() {
        Name        = "Headache",
        Color       = "#ff0000",
        Description = "times when I had a headache",
        IsPublic    = true
    };

    public EventTypeCreateTests() {
        _createCommand = new EventTypeCreateCommand(EventType: _createEventTypeRequest, 
                                                    SaveChanges: true,
                                                    Context: OperatorContext);
    }

    [Fact]
    public async Task Create_Given_EventType_With_AnalysisResults_Should_Not_Save_Them_To_DB() {
        var beforeSend = DateTime.UtcNow;
        _createCommand.EventType.AnalysisResults = new List<StatisticsForEventType> {
            new () {
                EntityId     = _createCommand.EventType.Id,
                AnalysisCode = FeatureCodes.GAP_ANALYSIS,
                ResultJson   = "",
                StartedAt    = DateTime.UtcNow,
                FinishedAt   = DateTime.UtcNow.AddMinutes(1)
            }
        };
        var cqrsResult = await Mediator.Send(request: _createCommand);

        cqrsResult.CheckSuccessfullyCreatedEntity(operatorId: OperatorUserId, beforeSend: beforeSend);
        
        TestData.DbContext.StatisticsForEventTypes.ToList().Should().BeEmpty();
    }
    
    [Fact]
    public async Task Create_EventType() {
        var beforeSend = DateTime.UtcNow;
        var cqrsResult = await Mediator.Send(request: _createCommand);

        cqrsResult.CheckSuccessfullyCreatedEntity(operatorId: OperatorUserId, beforeSend: beforeSend);

        cqrsResult.Result!.Name.Should().Be(expected: _createEventTypeRequest.Name);
        cqrsResult.Result.Color.Should().Be(expected: _createEventTypeRequest.Color);
        cqrsResult.Result.Description.Should().Be(expected: _createEventTypeRequest.Description);
        cqrsResult.Result.IsPublic.Should().Be(expected: _createEventTypeRequest.IsPublic);
    }

    [Fact]
    public async Task Create_EventType_With_Same_Name() {
        var createdEntities = TestData.Events.CreateManyEventTypes(count: 1, userId: OperatorUserId);
        _createEventTypeRequest.Name = createdEntities.First().Value.Name;

        var cqrsResult = await Mediator.Send(request: _createCommand);

        cqrsResult.CheckCreationOfSameEntity();
    }

    [Fact]
    public async Task Create_Already_Deleted_Entity() {
        var createdEntities = TestData.Events.CreateManyEventTypes(count: 1, userId: OperatorUserId, isDeleted: true);
        _createEventTypeRequest.Name = createdEntities.First().Value.Name;

        var cqrsResult = await Mediator.Send(request: _createCommand);

        cqrsResult.CheckUpdateOrCreationOfSoftDeletedEntity();
        cqrsResult.Result!.Name.Should().Be(expected: _createEventTypeRequest.Name);
    }
    
    [Fact]
    public async Task Given_EventType_With_Parent_Should_Setup_TreeNodePath() {
        var parent    = TestData.Events.CreateEventType(OperatorUserId, "Parent Test Event Type");
        _createCommand.EventType.ParentId = parent.Bl.Id;
        
        var cqrsResult = await Mediator.Send(request: _createCommand);
        
        cqrsResult.Should().NotBeNull();
        cqrsResult.Result.Should().NotBeNull();
        var createdEventType = TestData.DbContext.EventTypes.FirstOrDefault(x => x.Id == cqrsResult.Result!.Id);
        createdEventType.Should().NotBeNull();
        createdEventType!.TreeNodePath.ToString().Should().Be($"{parent.Bl.Id}.{cqrsResult.Result!.Id}");
    }
    
    [Fact]
    public async Task Given_Not_Existing_ParentId_Should_Set_Parent_To_Null() {
        _createCommand.EventType.ParentId = new Random().NextInt64();
        
        var cqrsResult = await Mediator.Send(request: _createCommand);
        
        cqrsResult.Should().NotBeNull();
        cqrsResult.Result.Should().NotBeNull();
        var createdEventType = TestData.DbContext.EventTypes.FirstOrDefault(x => x.Id == cqrsResult.Result!.Id);
        createdEventType.Should().NotBeNull();
        createdEventType!.ParentId.Should().BeNull();
        createdEventType.TreeNodePath.ToString().Should().Be(createdEventType.Id.ToString());
    }
}