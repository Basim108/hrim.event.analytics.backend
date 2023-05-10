using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Api.Tests.Infrastructure.AssertHelpers;
using Hrim.Event.Analytics.Api.V1.Models;
using Hrimsoft.Core.Extensions;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Events;

[ExcludeFromCodeCoverage]
public class DurationEventCreateTests : BaseCqrsTests
{
    private readonly DurationEventCreateCommand _createCommand;
    private readonly DurationEventCreateRequest _createRequest;
    private readonly UserEventType _eventType;

    public DurationEventCreateTests()
    {
        _eventType = TestData.Events.CreateEventType(OperatorUserId, $"Headache-{Guid.NewGuid()}");
        _createRequest = new DurationEventCreateRequest
        {
            StartedAt = DateTimeOffset.Now.AddHours(-1),
            FinishedAt = DateTimeOffset.Now,
            EventTypeId = _eventType.Id
        };
        _createCommand = new DurationEventCreateCommand(_createRequest, true, OperatorContext);
    }

    [Fact]
    public async Task Create_DurationEvent()
    {
        var beforeSend = DateTime.UtcNow;

        var cqrsResult = await Mediator.Send(_createCommand);

        cqrsResult.CheckSuccessfullyCreatedEntity(OperatorUserId, beforeSend);
        cqrsResult.Result!.StartedAt.Should().Be(_createRequest.StartedAt.TruncateToMilliseconds());
        cqrsResult.Result!.FinishedAt.Should().Be(_createRequest.FinishedAt!.Value.TruncateToMilliseconds());
    }

    [Fact]
    public async Task Create_DurationEvent_With_Same_StartedAt_And_EventType()
    {
        var dbEntity = TestData.Events.CreateDurationEvent(OperatorUserId, _eventType.Id);
        var createRequest = new DurationEventCreateRequest();
        dbEntity.CopyTo(createRequest);
        var command = new DurationEventCreateCommand(createRequest, true, OperatorContext);
        var cqrsResult = await Mediator.Send(command);

        cqrsResult.CheckCreationOfSameEntity();
    }

    [Fact]
    public async Task Create_DurationEvent_With_Same_But_Soft_Deleted_OccurredAt()
    {
        var dbEntity = TestData.Events.CreateDurationEvent(OperatorUserId, _eventType.Id, true);
        var createRequest = new DurationEventCreateRequest();
        dbEntity.CopyTo(createRequest);

        var command = new DurationEventCreateCommand(createRequest, true, OperatorContext);
        var cqrsResult = await Mediator.Send(command);

        cqrsResult.CheckUpdateOrCreationOfSoftDeletedEntity();
    }

    // TODO: Should work with intervals intersections: add intersection_behaviour
    // [Fact]
    // public async Task Create_DurationEvent_With_Intervals_Intersection_Right_Border() {
    //     var dbEntity      = TestData.Events.CreateDurationEvent(OperatorUserId, _eventType.Id);
    //     var createRequest = new DurationEventCreateRequest();
    //     dbEntity.CopyTo(createRequest);
    //     createRequest.StartedAt = createRequest.StartedAt.AddMinutes(5);
    //     createRequest.FinishedAt = createRequest.FinishedAt!.Value.AddMinutes(5);
    //     
    //     var command    = new DurationEventCreateCommand(createRequest, SaveChanges: true, OperatorContext);
    //     var cqrsResult = await Mediator.Send(command);
    //
    //     cqrsResult.CheckCreationOfSameEntity();
    // }
    //
    // [Fact]
    // public async Task Create_DurationEvent_With_Intervals_Intersection_Left_Border() {
    //     var dbEntity      = TestData.Events.CreateDurationEvent(OperatorUserId, _eventType.Id);
    //     var createRequest = new DurationEventCreateRequest();
    //     dbEntity.CopyTo(createRequest);
    //     createRequest.FinishedAt = createRequest.StartedAt.AddMinutes(5);
    //     createRequest.StartedAt  = createRequest.StartedAt.AddMinutes(-5);
    //     
    //     var command    = new DurationEventCreateCommand(createRequest, SaveChanges: true, OperatorContext);
    //     var cqrsResult = await Mediator.Send(command);
    //
    //     cqrsResult.CheckCreationOfSameEntity();
    // }
    //
    // [Fact]
    // public async Task Create_DurationEvent_With_Interval_Inside_Existed() {
    //     var dbEntity      = TestData.Events.CreateDurationEvent(OperatorUserId, _eventType.Id);
    //     var createRequest = new DurationEventCreateRequest();
    //     dbEntity.CopyTo(createRequest);
    //     createRequest.StartedAt  = createRequest.StartedAt.AddMinutes(5);
    //     createRequest.FinishedAt = createRequest.FinishedAt!.Value.AddMinutes(-5);
    //     
    //     var command    = new DurationEventCreateCommand(createRequest, SaveChanges: true, OperatorContext);
    //     var cqrsResult = await Mediator.Send(command);
    //
    //     cqrsResult.CheckCreationOfSameEntity();
    // }

    // TODO: Should return two intervals: [before existed][existed][after existed]
    // [Fact]
    // public async Task Create_DurationEvent_With_Interval_Includes_Existed() {
    //     var dbEntity      = TestData.Events.CreateDurationEvent(OperatorUserId, _eventType.Id);
    //     var createRequest = new DurationEventCreateRequest();
    //     dbEntity.CopyTo(createRequest);
    //     createRequest.StartedAt  = createRequest.StartedAt.AddMinutes(-5);
    //     createRequest.FinishedAt = createRequest.FinishedAt!.Value.AddMinutes(5);
    //     
    //     var command    = new DurationEventCreateCommand(createRequest, SaveChanges: true, OperatorContext);
    //     var cqrsResult = await Mediator.Send(command);
    //
    //     cqrsResult.CheckCreationOfSameEntity();
    // }
}