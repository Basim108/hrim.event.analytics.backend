using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Api.Tests.Infrastructure.AssertHelpers;
using Hrim.Event.Analytics.Api.V1.Models;
using Hrimsoft.Core.Extensions;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Events;

[ExcludeFromCodeCoverage]
public class OccurrenceEventUpdateTests: BaseCqrsTests {
    private readonly OccurrenceEventUpdateRequest _updateRequest;
    private readonly UserEventType                _eventType;

    public OccurrenceEventUpdateTests() {
        _eventType = TestData.CreateEventType(OperatorContext.UserId, $"Nice practice-{Guid.NewGuid()}");
        _updateRequest = new OccurrenceEventUpdateRequest {
            OccurredAt  = DateTimeOffset.Now,
            EventTypeId = _eventType.Id
        };
    }

    [Fact]
    public async Task Update_OccurrenceEvent() {
        var dbEvent = TestData.CreateOccurrenceEvent(OperatorContext.UserId,
                                                     _eventType.Id, 
                                                     isDeleted: false, 
                                                     _updateRequest.OccurredAt);

        var forUpdate = new OccurrenceEventUpdateRequest();
        dbEvent.CopyTo(forUpdate);
        forUpdate.OccurredAt = DateTimeOffset.Now.AddHours(-1);
        var beforeSend    = DateTime.UtcNow;
        var updateCommand = new OccurrenceEventUpdateCommand(forUpdate, SaveChanges: true, OperatorContext);

        var cqrsResult = await Mediator.Send(updateCommand);

        cqrsResult.CheckSuccessfullyUpdatedEntity(OperatorContext.UserId, forUpdate, beforeSend);
        cqrsResult.Result!.OccurredAt.Should().Be(forUpdate.OccurredAt.TruncateToMilliseconds());
    }

    [Fact]
    public async Task Update_OccurrenceEvent_With_Same_But_Soft_Deleted_OccurredAt() {
        var dbEvent = TestData.CreateOccurrenceEvent(OperatorContext.UserId,
                                                     _eventType.Id, 
                                                     isDeleted: true, 
                                                     _updateRequest.OccurredAt);

        var forUpdate = new OccurrenceEventUpdateRequest();
        dbEvent.CopyTo(forUpdate);
        forUpdate.IsDeleted = null;
        var updateCommand = new OccurrenceEventUpdateCommand(forUpdate, SaveChanges: true, OperatorContext);
        
        var cqrsResult = await Mediator.Send(updateCommand);

        cqrsResult.CheckUpdateOrCreationOfSoftDeletedEntity();
    }
    
    [Fact]
    public async Task Update_OccurrenceEvent_With_Wrong_ConcurrentToken() {
        var dbEvent = TestData.CreateOccurrenceEvent(OperatorContext.UserId,
                                                     _eventType.Id, 
                                                     isDeleted: false, 
                                                     _updateRequest.OccurredAt);

        var forUpdate = new OccurrenceEventUpdateRequest();
        dbEvent.CopyTo(forUpdate);
        forUpdate.ConcurrentToken = 4;
        var updateCommand = new OccurrenceEventUpdateCommand(forUpdate, SaveChanges: true, OperatorContext);
        
        var cqrsResult = await Mediator.Send(updateCommand);

        cqrsResult.CheckConcurrentConflictUpdate(forUpdate);
    }
}