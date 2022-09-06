using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Api.Tests.Infrastructure.AssertHelpers;
using Hrim.Event.Analytics.Api.V1.Models;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.EventTypes;

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

        cqrsResult.CheckSuccessfullyCreatedEntity(OperatorContext.UserId, beforeSend);

        cqrsResult.Result!.Name.Should().Be(_createEventTypeRequest.Name);
        cqrsResult.Result.Color.Should().Be(_createEventTypeRequest.Color);
        cqrsResult.Result.Description.Should().Be(_createEventTypeRequest.Description);
        cqrsResult.Result.IsPublic.Should().Be(_createEventTypeRequest.IsPublic);
    }

    [Fact]
    public async Task Create_EventType_With_Same_Name() {
        var createdEntities = TestData.Events.CreateManyEventTypes(1, OperatorContext.UserId);
        _createEventTypeRequest.Name = createdEntities.First().Value.Name;

        var cqrsResult = await Mediator.Send(_createCommand);
        
        cqrsResult.CheckCreationOfSameEntity();
    }

    [Fact]
    public async Task Create_Already_Deleted_Entity() {
        var createdEntities = TestData.Events.CreateManyEventTypes(1, OperatorContext.UserId, isDeleted: true);
        _createEventTypeRequest.Name = createdEntities.First().Value.Name;

        var cqrsResult = await Mediator.Send(_createCommand);
        
        cqrsResult.CheckUpdateOrCreationOfSoftDeletedEntity();
        cqrsResult.Result!.Name.Should().Be(_createEventTypeRequest.Name);
    }
}