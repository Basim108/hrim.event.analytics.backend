using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;
using Hrim.Event.Analytics.Api.V1.Models;

namespace Hrim.Event.Analytics.Api.Tests.Cqrs.EventTypes;

[ExcludeFromCodeCoverage]
public class EventTypeCreateTests: BaseCqrsTests {
    private readonly EventTypeCreateCommand _createCommand;

    public EventTypeCreateTests() {
        _createCommand = new EventTypeCreateCommand(_createEventTypeRequest, SaveChanges: true, OperatorContext);
    }

    /// <summary> Correct create event type request  </summary>
    private readonly CreateEventTypeRequest _createEventTypeRequest = new() {
        Name        = "Headache",
        Color       = "#ff0000",
        Description = "times when I had a headache",
        IsPublic    = true
    };

    [Fact]
    public async Task Create_EventType() {
        var beforeSend = DateTime.UtcNow;
        var cqrsResult = await Mediator.Send(_createCommand);
        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(CqrsResultCode.Created);
        var resultEventType = cqrsResult.Result;
        resultEventType.Should().NotBeNull();
        resultEventType!.Id.Should().NotBeEmpty();
        resultEventType.CreatedById.Should().Be(OperatorContext.UserId);
        resultEventType.CreatedAt.Should().BeAfter(beforeSend);
        resultEventType.UpdatedAt.Should().BeNull();
        resultEventType.IsDeleted.Should().BeNull();
        resultEventType.ConcurrentToken.Should().Be(1);

        resultEventType.Name.Should().Be(_createEventTypeRequest.Name);
        resultEventType.Color.Should().Be(_createEventTypeRequest.Color);
        resultEventType.Description.Should().Be(_createEventTypeRequest.Description);
        resultEventType.IsPublic.Should().Be(_createEventTypeRequest.IsPublic);
    }

    [Fact]
    public async Task Create_EventType_With_Same_Name() {
        var createdEntities = TestData.CreateManyEventTypes(1, OperatorContext.UserId);
        _createEventTypeRequest.Name = createdEntities.First().Value.Name;

        var cqrsResult = await Mediator.Send(_createCommand);
        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(CqrsResultCode.Conflict);
    }

    [Fact]
    public async Task Create_Already_Deleted_Entity() {
        var createdEntities = TestData.CreateManyEventTypes(1, OperatorContext.UserId, isDeleted: true);
        _createEventTypeRequest.Name = createdEntities.First().Value.Name;

        var cqrsResult = await Mediator.Send(_createCommand);
        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(CqrsResultCode.EntityIsDeleted);
        cqrsResult.Info.Should().BeNull();

        cqrsResult.Result.Should().NotBeNull();
        cqrsResult.Result!.Name.Should().Be(_createEventTypeRequest.Name);
        cqrsResult.Result!.IsDeleted.Should().BeTrue();
    }
}