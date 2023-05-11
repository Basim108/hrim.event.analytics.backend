using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Api.Tests.Infrastructure.AssertHelpers;
using Hrim.Event.Analytics.Api.V1.Models;
using Hrimsoft.Core.Extensions;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Events;

[ExcludeFromCodeCoverage]
public class OccurrenceEventCreateTests: BaseCqrsTests
{
    private readonly OccurrenceEventCreateCommand _createCommand;
    private readonly OccurrenceEventCreateRequest _createRequest;
    private readonly UserEventType                _eventType;

    public OccurrenceEventCreateTests() {
        _eventType = TestData.Events.CreateEventType(userId: OperatorUserId, $"Nice practice-{Guid.NewGuid()}");
        _createRequest = new OccurrenceEventCreateRequest {
            OccurredAt  = DateTimeOffset.Now,
            EventTypeId = _eventType.Id
        };
        _createCommand = new OccurrenceEventCreateCommand(EventInfo: _createRequest, SaveChanges: true, Context: OperatorContext);
    }

    [Fact]
    public async Task Create_OccurrenceEvent() {
        var beforeSend = DateTime.UtcNow;

        var cqrsResult = await Mediator.Send(request: _createCommand);

        cqrsResult.CheckSuccessfullyCreatedEntity(operatorId: OperatorUserId, beforeSend: beforeSend);
        cqrsResult.Result!.OccurredAt.Should().Be(_createRequest.OccurredAt.TruncateToMilliseconds());
    }

    [Fact]
    public async Task Create_OccurrenceEvent_With_Same_OccurredAt_And_EventType() {
        var dbEntity =
            TestData.Events.CreateOccurrenceEvent(userId: OperatorUserId,
                                                  eventTypeId: _eventType.Id,
                                                  isDeleted: false,
                                                  occurredAt: _createRequest.OccurredAt);
        var createRequest = new OccurrenceEventCreateRequest();
        dbEntity.CopyTo(another: createRequest);
        var command    = new OccurrenceEventCreateCommand(EventInfo: createRequest, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        cqrsResult.CheckCreationOfSameEntity();
    }

    [Fact]
    public async Task Create_OccurrenceEvent_With_Same_But_Soft_Deleted_OccurredAt() {
        TestData.Events.CreateOccurrenceEvent(userId: OperatorUserId, eventTypeId: _eventType.Id, isDeleted: true, occurredAt: _createRequest.OccurredAt);
        var dbEntity =
            TestData.Events.CreateOccurrenceEvent(userId: OperatorUserId,
                                                  eventTypeId: _eventType.Id,
                                                  isDeleted: false,
                                                  occurredAt: _createRequest.OccurredAt);
        var createRequest = new OccurrenceEventCreateRequest();
        dbEntity.CopyTo(another: createRequest);
        createRequest.IsDeleted = null;
        var command = new OccurrenceEventCreateCommand(EventInfo: createRequest, SaveChanges: true, Context: OperatorContext);

        var cqrsResult = await Mediator.Send(request: command);

        cqrsResult.CheckUpdateOrCreationOfSoftDeletedEntity();
    }
}