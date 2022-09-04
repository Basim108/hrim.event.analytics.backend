using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;

namespace Hrim.Event.Analytics.Api.Tests.Cqrs.EventTypes;

[ExcludeFromCodeCoverage]
public class EventTypeCreateTests: BaseCqrsTests {
    private readonly EventTypeCreateCommand _createCommand;

    public EventTypeCreateTests() {
        _createCommand = new EventTypeCreateCommand(TestData.CreateEventTypeRequest, SaveChanges: true, OperatorContext);
    }

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

        resultEventType.Name.Should().Be(TestData.CreateEventTypeRequest.Name);
        resultEventType.Color.Should().Be(TestData.CreateEventTypeRequest.Color);
        resultEventType.Description.Should().Be(TestData.CreateEventTypeRequest.Description);
        resultEventType.IsPublic.Should().Be(TestData.CreateEventTypeRequest.IsPublic);
    }

    [Fact]
    public async Task Create_EventType_With_Same_Name() {
        var createdEntities = TestData.CreateManyEventTypes(1, OperatorContext.UserId);
        TestData.CreateEventTypeRequest.Name = createdEntities.First().Value.Name;

        var cqrsResult = await Mediator.Send(_createCommand);
        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(CqrsResultCode.Conflict);
    }

    [Fact]
    public async Task Create_Already_Deleted_Entity() {
        var createdEntities = TestData.CreateManyEventTypes(1, OperatorContext.UserId, isDeleted: true);
        TestData.CreateEventTypeRequest.Name = createdEntities.First().Value.Name;

        var cqrsResult = await Mediator.Send(_createCommand);
        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(CqrsResultCode.EntityIsDeleted);
        cqrsResult.Info.Should().BeNull();

        cqrsResult.Result.Should().NotBeNull();
        cqrsResult.Result!.Name.Should().Be(TestData.CreateEventTypeRequest.Name);
        cqrsResult.Result!.IsDeleted.Should().BeTrue();
    }
}