using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Api.Tests.Infrastructure.AssertHelpers;
using Hrim.Event.Analytics.Api.V1.Models;
using Hrimsoft.Core.Extensions;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Events;

[ExcludeFromCodeCoverage]
public class OccurrenceEventUpdateTests: BaseCqrsTests
{
    private readonly EventType                _eventType;
    private readonly OccurrenceEventUpdateRequest _updateRequest;

    public OccurrenceEventUpdateTests() {
        _eventType = TestData.Events.CreateEventType(userId: OperatorUserId, $"Nice practice-{Guid.NewGuid()}").Bl;
        _updateRequest = new OccurrenceEventUpdateRequest {
            OccurredAt  = DateTimeOffset.Now,
            EventTypeId = _eventType.Id
        };
    }

    [Fact]
    public async Task Update_Given_Props_OccurrenceEvent_Should_Save_It() {
        var dbEvent = TestData.Events.CreateOccurrenceEvent(userId: OperatorUserId, eventTypeId: _eventType.Id);

        var forUpdate = new OccurrenceEventUpdateRequest();
        dbEvent.CopyTo(another: forUpdate);
        forUpdate.Props = new Dictionary<string, string>() {
            { "notes", "this is a test note" }
        };
        var updateCommand = new OccurrenceEventUpdateCommand(EventInfo: forUpdate, SaveChanges: true, Context: OperatorContext);

        await Mediator.Send(request: updateCommand);

        var savedEvent = TestData.DbContext.OccurrenceEvents.FirstOrDefault(x => x.Id == forUpdate.Id);
        savedEvent.Should().NotBeNull();
        savedEvent!.Props.Should().NotBeEmpty();
        savedEvent.Props!.ContainsKey("notes").Should().BeTrue();
    }
    
    [Fact]
    public async Task Update_OccurrenceEvent() {
        var dbEvent = TestData.Events.CreateOccurrenceEvent(userId: OperatorUserId,
                                                            eventTypeId: _eventType.Id,
                                                            isDeleted: false,
                                                            occurredAt: _updateRequest.OccurredAt);

        var forUpdate = new OccurrenceEventUpdateRequest();
        dbEvent.CopyTo(another: forUpdate);
        forUpdate.OccurredAt = DateTimeOffset.Now.AddHours(hours: -1);
        var beforeSend    = DateTime.UtcNow;
        var updateCommand = new OccurrenceEventUpdateCommand(EventInfo: forUpdate, SaveChanges: true, Context: OperatorContext);

        var cqrsResult = await Mediator.Send(request: updateCommand);

        cqrsResult.CheckSuccessfullyUpdatedEntity(operatorId: OperatorUserId, forUpdate: forUpdate, beforeSend: beforeSend);
        cqrsResult.Result!.OccurredAt.Should().Be(forUpdate.OccurredAt.TruncateToMilliseconds());
    }

    [Fact]
    public async Task Update_OccurrenceEvent_With_Same_But_Soft_Deleted_OccurredAt() {
        var dbEvent = TestData.Events.CreateOccurrenceEvent(userId: OperatorUserId,
                                                            eventTypeId: _eventType.Id,
                                                            isDeleted: true,
                                                            occurredAt: _updateRequest.OccurredAt);

        var forUpdate = new OccurrenceEventUpdateRequest();
        dbEvent.CopyTo(another: forUpdate);
        forUpdate.IsDeleted = null;
        var updateCommand = new OccurrenceEventUpdateCommand(EventInfo: forUpdate, SaveChanges: true, Context: OperatorContext);

        var cqrsResult = await Mediator.Send(request: updateCommand);

        cqrsResult.CheckUpdateOrCreationOfSoftDeletedEntity();
    }

    [Fact]
    public async Task Update_OccurrenceEvent_With_Wrong_ConcurrentToken() {
        var dbEvent = TestData.Events.CreateOccurrenceEvent(userId: OperatorUserId,
                                                            eventTypeId: _eventType.Id,
                                                            isDeleted: false,
                                                            occurredAt: _updateRequest.OccurredAt);

        var forUpdate = new OccurrenceEventUpdateRequest();
        dbEvent.CopyTo(another: forUpdate);
        forUpdate.ConcurrentToken = 4;
        var updateCommand = new OccurrenceEventUpdateCommand(EventInfo: forUpdate, SaveChanges: true, Context: OperatorContext);

        var cqrsResult = await Mediator.Send(request: updateCommand);

        cqrsResult.CheckConcurrentConflictUpdate(forUpdate: forUpdate);
    }
}