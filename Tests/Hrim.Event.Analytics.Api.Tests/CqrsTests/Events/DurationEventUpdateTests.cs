using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Api.Tests.Infrastructure.AssertHelpers;
using Hrim.Event.Analytics.Api.V1.Models;
using Hrimsoft.Core.Extensions;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Events;

[ExcludeFromCodeCoverage]
public class DurationEventUpdateTests : BaseCqrsTests
{
    private readonly UserEventType _eventType;

    public DurationEventUpdateTests()
    {
        _eventType = TestData.Events.CreateEventType(OperatorContext.UserId, $"Headache-{Guid.NewGuid()}");
    }

    [Fact]
    public async Task Update_StartedAt()
    {
        var dbEvent = TestData.Events.CreateDurationEvent(OperatorContext.UserId, _eventType.Id);

        var forUpdate = new DurationEventUpdateRequest();
        dbEvent.CopyTo(forUpdate);
        forUpdate.StartedAt = DateTimeOffset.Now.AddHours(-1);
        var beforeSend = DateTime.UtcNow;
        var updateCommand = new DurationEventUpdateCommand(forUpdate, true, OperatorContext);

        var cqrsResult = await Mediator.Send(updateCommand);

        cqrsResult.CheckSuccessfullyUpdatedEntity(OperatorContext.UserId, forUpdate, beforeSend);
        cqrsResult.Result!.StartedAt.Should().Be(forUpdate.StartedAt.TruncateToMilliseconds());
    }

    [Fact]
    public async Task Update_StartedAt_With_Same_But_Soft_Deleted_OccurredAt()
    {
        var dbEvent = TestData.Events.CreateDurationEvent(OperatorContext.UserId, _eventType.Id, true);

        var forUpdate = new DurationEventUpdateRequest();
        dbEvent.CopyTo(forUpdate);
        forUpdate.IsDeleted = null;
        var updateCommand = new DurationEventUpdateCommand(forUpdate, true, OperatorContext);

        var cqrsResult = await Mediator.Send(updateCommand);

        cqrsResult.CheckUpdateOrCreationOfSoftDeletedEntity();
    }

    [Fact]
    public async Task Update_StartedAt_With_Wrong_ConcurrentToken()
    {
        var dbEvent = TestData.Events.CreateDurationEvent(OperatorContext.UserId, _eventType.Id);

        var forUpdate = new DurationEventUpdateRequest();
        dbEvent.CopyTo(forUpdate);
        forUpdate.ConcurrentToken = 4;
        var updateCommand = new DurationEventUpdateCommand(forUpdate, true, OperatorContext);

        var cqrsResult = await Mediator.Send(updateCommand);

        cqrsResult.CheckConcurrentConflictUpdate(forUpdate);
    }
}