using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.Entity;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.EfCore.DbEntities.Events;
using Microsoft.Extensions.DependencyInjection;
using EventType = Hrim.Event.Analytics.Abstractions.Entities.EventTypes.EventType;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Entity;

[ExcludeFromCodeCoverage]
public class RestoreEntityTests: BaseCqrsTests
{
    private readonly IMapper _mapper;

    public RestoreEntityTests() { _mapper = ServiceProvider.GetRequiredService<IMapper>(); }

    [Fact]
    public async Task EventType_Should_Forbid() {
        var anotherId = new Random().NextInt64();
        TestData.Users.EnsureUserExistence(id: anotherId);
        var headache = TestData.Events.CreateEventType(userId: anotherId, $"Headache-{Guid.NewGuid()}", isDeleted: true).Bl;

        var command    = new RestoreLongEntityCommand<EventType>(Id: headache.Id, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.Forbidden);
    }

    [Fact]
    public async Task DurationEvent_Should_Forbid() {
        var anotherId = new Random().NextInt64();
        TestData.Users.EnsureUserExistence(id: anotherId);
        var headache      = TestData.Events.CreateEventType(userId: anotherId, $"Headache-{Guid.NewGuid()}").Bl;
        var durationEvent = TestData.Events.CreateDurationEvent(userId: anotherId, eventTypeId: headache.Id, isDeleted: true);

        var command    = new RestoreLongEntityCommand<DurationEvent>(Id: durationEvent.Id, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.Forbidden);
    }

    [Fact]
    public async Task OccurrenceEvent_Should_Forbid() {
        var anotherId = new Random().NextInt64();
        TestData.Users.EnsureUserExistence(id: anotherId);
        var nicePractice    = TestData.Events.CreateEventType(userId: anotherId, $"Nice Practice-{Guid.NewGuid()}").Bl;
        var occurrenceEvent = TestData.Events.CreateOccurrenceEvent(userId: anotherId, eventTypeId: nicePractice.Id, isDeleted: true);

        var command    = new RestoreLongEntityCommand<OccurrenceEvent>(Id: occurrenceEvent.Id, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.Forbidden);
    }

    [Fact]
    public async Task User_NotFound() {
        var entityId   = new Random().NextInt64();
        var command    = new RestoreLongEntityCommand<HrimUser>(Id: entityId, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.NotFound);
    }

    [Fact]
    public async Task EventType_NotFound() {
        var entityId   = new Random().NextInt64();
        var command    = new RestoreLongEntityCommand<EventType>(Id: entityId, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.NotFound);
    }

    [Fact]
    public async Task DurationEvent_NotFound() {
        var entityId   = new Random().NextInt64();
        var command    = new RestoreLongEntityCommand<DurationEvent>(Id: entityId, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.NotFound);
    }

    [Fact]
    public async Task OccurrenceEvent_NotFound() {
        var entityId   = new Random().NextInt64();
        var command    = new RestoreLongEntityCommand<OccurrenceEvent>(Id: entityId, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.NotFound);
    }

    [Fact]
    public async Task Tag_NotFound() {
        var entityId   = new Random().NextInt64();
        var command    = new RestoreLongEntityCommand<HrimTag>(Id: entityId, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.NotFound);
    }

    [Fact]
    public async Task User_Should_Restore() {
        var entityId = new Random().NextInt64();
        TestData.Users.EnsureUserExistence(id: entityId, isDeleted: true);
        var command    = new RestoreLongEntityCommand<HrimUser>(Id: entityId, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        CheckRestoredEntity(entityId, cqrsResult, queryable: TestData.DbContext.HrimUsers);
    }

    [Fact]
    public async Task EventType_Should_Restore() {
        var headache = TestData.Events.CreateEventType(userId: OperatorUserId, $"Headache-{Guid.NewGuid()}", isDeleted: true).Bl;

        var command    = new RestoreLongEntityCommand<EventType>(Id: headache.Id, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.Ok);
        var restored = cqrsResult.Result;
        restored.Should().NotBeNull();
        restored!.Id.Should().Be(expected: headache.Id);
        restored.IsDeleted.Should().BeFalse();
        restored.ConcurrentToken.Should().Be(expected: 2);
        var entity = TestData.DbContext.EventTypes.First(x => x.Id == headache.Id);
        entity.IsDeleted.Should().BeFalse();
        entity.ConcurrentToken.Should().Be(expected: 2);
    }

    [Fact]
    public async Task DurationEvent_Should_Restore() {
        var headache      = TestData.Events.CreateEventType(userId: OperatorUserId, $"Headache-{Guid.NewGuid()}").Bl;
        var durationEvent = TestData.Events.CreateDurationEvent(userId: OperatorUserId, eventTypeId: headache.Id, isDeleted: true);

        var command    = new RestoreLongEntityCommand<DurationEvent>(Id: durationEvent.Id, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        var dbEntity     = _mapper.Map<DbDurationEvent>(source: cqrsResult.Result);
        var dbCqrsResult = new CqrsResult<DbDurationEvent?>(Result: dbEntity, StatusCode: cqrsResult.StatusCode, Info: cqrsResult.Info);
        CheckRestoredEntity(entityId: durationEvent.Id, cqrsResult: dbCqrsResult, queryable: TestData.DbContext.DurationEvents);
    }

    [Fact]
    public async Task OccurrenceEvent_Should_Restore() {
        var nicePractice      = TestData.Events.CreateEventType(userId: OperatorUserId, $"Nice practice-{Guid.NewGuid()}").Bl;
        var nicePracticeEvent = TestData.Events.CreateOccurrenceEvent(userId: OperatorUserId, eventTypeId: nicePractice.Id, isDeleted: true);

        var command    = new RestoreLongEntityCommand<OccurrenceEvent>(Id: nicePracticeEvent.Id, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        var dbEntity     = _mapper.Map<DbOccurrenceEvent>(source: cqrsResult.Result);
        var dbCqrsResult = new CqrsResult<DbOccurrenceEvent?>(Result: dbEntity, StatusCode: cqrsResult.StatusCode, Info: cqrsResult.Info);
        CheckRestoredEntity(entityId: nicePracticeEvent.Id, cqrsResult: dbCqrsResult, queryable: TestData.DbContext.OccurrenceEvents);
    }

    [Fact]
    public async Task Not_Deleted_User_Should_Recognize() {
        var entityId = new Random().NextInt64();
        TestData.Users.EnsureUserExistence(id: entityId);
        var command    = new RestoreLongEntityCommand<HrimUser>(Id: entityId, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.EntityIsNotDeleted);
    }

    [Fact]
    public async Task Not_Deleted_EventType_Should_Recognize() {
        var headache = TestData.Events.CreateEventType(userId: OperatorUserId, $"Headache-{Guid.NewGuid()}").Bl;

        var command    = new RestoreLongEntityCommand<EventType>(Id: headache.Id, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.EntityIsNotDeleted);
    }

    [Fact]
    public async Task Not_Deleted_DurationEvent_Should_Recognize() {
        var headache      = TestData.Events.CreateEventType(userId: OperatorUserId, $"Headache-{Guid.NewGuid()}").Bl;
        var durationEvent = TestData.Events.CreateDurationEvent(userId: OperatorUserId, eventTypeId: headache.Id);

        var command    = new RestoreLongEntityCommand<DurationEvent>(Id: durationEvent.Id, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.EntityIsNotDeleted);
    }

    [Fact]
    public async Task Not_Deleted_OccurrenceEvent_Should_Recognized() {
        var nicePractice      = TestData.Events.CreateEventType(userId: OperatorUserId, $"Nice practice-{Guid.NewGuid()}").Bl;
        var nicePracticeEvent = TestData.Events.CreateOccurrenceEvent(userId: OperatorUserId, eventTypeId: nicePractice.Id);

        var command    = new RestoreLongEntityCommand<OccurrenceEvent>(Id: nicePracticeEvent.Id, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.EntityIsNotDeleted);
    }

    private static void CheckRestoredEntity<TEntity>(long                 entityId,
                                                     CqrsResult<TEntity?> cqrsResult,
                                                     IQueryable<TEntity>  queryable)
        where TEntity : HrimEntity<long> {
        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.Ok);
        var restored = cqrsResult.Result;
        restored.Should().NotBeNull();
        restored!.Id.Should().Be(expected: entityId);
        restored.IsDeleted.Should().BeFalse();
        restored.ConcurrentToken.Should().Be(expected: 2);
        var entity = queryable.First(x => x.Id == entityId);
        entity.IsDeleted.Should().BeFalse();
        entity.ConcurrentToken.Should().Be(expected: 2);
    }
}