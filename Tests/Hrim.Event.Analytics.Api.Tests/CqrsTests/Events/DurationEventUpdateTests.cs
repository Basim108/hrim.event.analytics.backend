using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Api.Tests.Infrastructure.AssertHelpers;
using Hrim.Event.Analytics.Api.V1.Models;
using Hrimsoft.Core.Extensions;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Events;

[ExcludeFromCodeCoverage]
public class DurationEventUpdateTests: BaseCqrsTests
{
    private readonly UserEventType _eventType;

    public DurationEventUpdateTests() { _eventType = TestData.Events.CreateEventType(userId: OperatorUserId, $"Headache-{Guid.NewGuid()}"); }

    [Fact]
    public async Task Update_Given_Props_DurationEvent_Should_Save_It() {
        var dbEvent = TestData.Events.CreateDurationEvent(userId: OperatorUserId, eventTypeId: _eventType.Id);

        var forUpdate = new DurationEventUpdateRequest();
        dbEvent.CopyTo(another: forUpdate);
        forUpdate.Props = new Dictionary<string, string>() {
            { "notes", "this is a test note" }
        };
        var beforeSend    = DateTime.UtcNow;
        var updateCommand = new DurationEventUpdateCommand(EventInfo: forUpdate, SaveChanges: true, Context: OperatorContext);

        var cqrsResult = await Mediator.Send(request: updateCommand);

        cqrsResult.CheckSuccessfullyUpdatedEntity(operatorId: OperatorUserId, forUpdate: forUpdate, beforeSend: beforeSend);
        var savedEvent = TestData.DbContext.DurationEvents.FirstOrDefault(x => x.Id == cqrsResult.Result!.Id);
        savedEvent.Should().NotBeNull();
        savedEvent!.Props.Should().NotBeEmpty();
        savedEvent.Props!.ContainsKey("notes").Should().BeTrue();
    }
    
    [Fact]
    public async Task Update_StartedAt() {
        var dbEvent = TestData.Events.CreateDurationEvent(userId: OperatorUserId, eventTypeId: _eventType.Id);

        var forUpdate = new DurationEventUpdateRequest();
        dbEvent.CopyTo(another: forUpdate);
        forUpdate.StartedAt = DateTimeOffset.Now.AddHours(hours: -1);
        var beforeSend    = DateTime.UtcNow;
        var updateCommand = new DurationEventUpdateCommand(EventInfo: forUpdate, SaveChanges: true, Context: OperatorContext);

        var cqrsResult = await Mediator.Send(request: updateCommand);

        cqrsResult.CheckSuccessfullyUpdatedEntity(operatorId: OperatorUserId, forUpdate: forUpdate, beforeSend: beforeSend);
        cqrsResult.Result!.StartedAt.Should().Be(forUpdate.StartedAt.TruncateToMilliseconds());
    }

    [Fact]
    public async Task Update_StartedAt_With_Same_But_Soft_Deleted_OccurredAt() {
        var dbEvent = TestData.Events.CreateDurationEvent(userId: OperatorUserId, eventTypeId: _eventType.Id, isDeleted: true);

        var forUpdate = new DurationEventUpdateRequest();
        dbEvent.CopyTo(another: forUpdate);
        forUpdate.IsDeleted = null;
        var updateCommand = new DurationEventUpdateCommand(EventInfo: forUpdate, SaveChanges: true, Context: OperatorContext);

        var cqrsResult = await Mediator.Send(request: updateCommand);

        cqrsResult.CheckUpdateOrCreationOfSoftDeletedEntity();
    }

    [Fact]
    public async Task Update_StartedAt_With_Wrong_ConcurrentToken() {
        var dbEvent = TestData.Events.CreateDurationEvent(userId: OperatorUserId, eventTypeId: _eventType.Id);

        var forUpdate = new DurationEventUpdateRequest();
        dbEvent.CopyTo(another: forUpdate);
        forUpdate.ConcurrentToken = 4;
        var updateCommand = new DurationEventUpdateCommand(EventInfo: forUpdate, SaveChanges: true, Context: OperatorContext);

        var cqrsResult = await Mediator.Send(request: updateCommand);

        cqrsResult.CheckConcurrentConflictUpdate(forUpdate: forUpdate);
    }
}