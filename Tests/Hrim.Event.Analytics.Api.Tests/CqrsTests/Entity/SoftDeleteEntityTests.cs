using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.Entity;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.EfCore.DbEntities.Events;
using Microsoft.Extensions.DependencyInjection;
using EventType = Hrim.Event.Analytics.Abstractions.Entities.EventTypes.EventType;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Entity;

[ExcludeFromCodeCoverage]
public class SoftDeleteEntityTests: BaseCqrsTests
{
    private readonly IMapper _mapper;

    public SoftDeleteEntityTests() { _mapper = ServiceProvider.GetRequiredService<IMapper>(); }

    [Fact]
    public async Task EventType_Should_Forbid() {
        var anotherId = new Random().NextInt64();
        TestData.Users.EnsureUserExistence(id: anotherId);
        var headache = TestData.Events.CreateEventType(userId: anotherId, $"Headache-{Guid.NewGuid()}").Bl;

        var command    = new SoftDeleteLongEntityCommand<EventType>(Id: headache.Id, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.Forbidden);
    }

    [Fact]
    public async Task DurationEvent_Should_Forbid() {
        var anotherId = new Random().NextInt64();
        TestData.Users.EnsureUserExistence(id: anotherId);
        var headache      = TestData.Events.CreateEventType(userId: anotherId, $"Headache-{Guid.NewGuid()}").Bl;
        var durationEvent = TestData.Events.CreateDurationEvent(userId: anotherId, eventTypeId: headache.Id);

        var command    = new SoftDeleteLongEntityCommand<DurationEvent>(Id: durationEvent.Id, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.Forbidden);
    }

    [Fact]
    public async Task OccurrenceEvent_Should_Forbid() {
        var anotherId = new Random().NextInt64();
        TestData.Users.EnsureUserExistence(id: anotherId);
        var nicePractice    = TestData.Events.CreateEventType(userId: anotherId, $"Nice Practice-{Guid.NewGuid()}").Bl;
        var occurrenceEvent = TestData.Events.CreateOccurrenceEvent(userId: anotherId, eventTypeId: nicePractice.Id);

        var command    = new SoftDeleteLongEntityCommand<OccurrenceEvent>(Id: occurrenceEvent.Id, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.Forbidden);
    }

    [Fact]
    public async Task User_NotFound() {
        var entityId   = new Random().NextInt64();
        var command    = new SoftDeleteLongEntityCommand<HrimUser>(Id: entityId, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.NotFound);
    }

    [Fact]
    public async Task EventType_NotFound() {
        var entityId   = new Random().NextInt64();
        var command    = new SoftDeleteLongEntityCommand<EventType>(Id: entityId, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.NotFound);
    }

    [Fact]
    public async Task DurationEvent_NotFound() {
        var entityId   = new Random().NextInt64();
        var command    = new SoftDeleteLongEntityCommand<DurationEvent>(Id: entityId, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.NotFound);
    }

    [Fact]
    public async Task OccurrenceEvent_NotFound() {
        var entityId   = new Random().NextInt64();
        var command    = new SoftDeleteLongEntityCommand<OccurrenceEvent>(Id: entityId, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.NotFound);
    }

    [Fact]
    public async Task Tag_NotFound() {
        var entityId   = new Random().NextInt64();
        var command    = new SoftDeleteLongEntityCommand<HrimTag>(Id: entityId, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.NotFound);
    }

    [Fact]
    public async Task User_Should_SoftDelete() {
        var entityId = new Random().NextInt64();
        TestData.Users.EnsureUserExistence(id: entityId);
        var command    = new SoftDeleteLongEntityCommand<HrimUser>(Id: entityId, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        CheckSoftDeletedEntity(entityId: entityId, cqrsResult: cqrsResult, queryable: TestData.DbContext.HrimUsers);
    }

    [Fact]
    public async Task EventType_Should_SoftDelete() {
        var headache = TestData.Events.CreateEventType(userId: OperatorUserId, $"Headache-{Guid.NewGuid()}").Bl;

        var command    = new SoftDeleteLongEntityCommand<EventType>(Id: headache.Id, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.Ok);
        var restored = cqrsResult.Result;
        restored.Should().NotBeNull();
        restored!.Id.Should().Be(expected: headache.Id);
        restored.IsDeleted.Should().BeTrue();
        restored.ConcurrentToken.Should().Be(expected: 2);
        var entity = TestData.DbContext.EventTypes.First(x => x.Id == headache.Id);
        entity.IsDeleted.Should().BeTrue();
        entity.ConcurrentToken.Should().Be(expected: 2);
    }

    [Fact]
    public async Task DurationEvent_Should_SoftDelete() {
        var headache      = TestData.Events.CreateEventType(userId: OperatorUserId, $"Headache-{Guid.NewGuid()}").Bl;
        var durationEvent = TestData.Events.CreateDurationEvent(userId: OperatorUserId, eventTypeId: headache.Id);

        var command    = new SoftDeleteLongEntityCommand<DurationEvent>(Id: durationEvent.Id, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        var dbEntity     = _mapper.Map<DbDurationEvent>(source: cqrsResult.Result);
        var dbCqrsResult = new CqrsResult<DbDurationEvent?>(Result: dbEntity, StatusCode: cqrsResult.StatusCode, Info: cqrsResult.Info);
        CheckSoftDeletedEntity(entityId: durationEvent.Id, cqrsResult: dbCqrsResult, queryable: TestData.DbContext.DurationEvents);
    }

    [Fact]
    public async Task OccurrenceEvent_Should_SoftDelete() {
        var nicePractice      = TestData.Events.CreateEventType(userId: OperatorUserId, $"Nice practice-{Guid.NewGuid()}").Bl;
        var nicePracticeEvent = TestData.Events.CreateOccurrenceEvent(userId: OperatorUserId, eventTypeId: nicePractice.Id);

        var command    = new SoftDeleteLongEntityCommand<OccurrenceEvent>(Id: nicePracticeEvent.Id, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        var dbEntity     = _mapper.Map<DbOccurrenceEvent>(source: cqrsResult.Result);
        var dbCqrsResult = new CqrsResult<DbOccurrenceEvent?>(Result: dbEntity, StatusCode: cqrsResult.StatusCode, Info: cqrsResult.Info);
        CheckSoftDeletedEntity(entityId: nicePracticeEvent.Id, cqrsResult: dbCqrsResult, queryable: TestData.DbContext.OccurrenceEvents);
    }

    [Fact]
    public async Task Deleted_User_Should_Recognize() {
        var entityId = new Random().NextInt64();
        TestData.Users.EnsureUserExistence(id: entityId, isDeleted: true);
        var command    = new SoftDeleteLongEntityCommand<HrimUser>(Id: entityId, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.EntityIsDeleted);
    }

    [Fact]
    public async Task Deleted_EventType_Should_Recognize() {
        var headache = TestData.Events.CreateEventType(userId: OperatorUserId, $"Headache-{Guid.NewGuid()}", isDeleted: true).Bl;

        var command    = new SoftDeleteLongEntityCommand<EventType>(Id: headache.Id, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.EntityIsDeleted);
    }

    [Fact]
    public async Task Deleted_DurationEvent_Should_Recognize() {
        var headache      = TestData.Events.CreateEventType(userId: OperatorUserId, $"Headache-{Guid.NewGuid()}").Bl;
        var durationEvent = TestData.Events.CreateDurationEvent(userId: OperatorUserId, eventTypeId: headache.Id, isDeleted: true);

        var command    = new SoftDeleteLongEntityCommand<DurationEvent>(Id: durationEvent.Id, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.EntityIsDeleted);
    }

    [Fact]
    public async Task Deleted_OccurrenceEvent_Should_Recognize() {
        var nicePractice      = TestData.Events.CreateEventType(userId: OperatorUserId, $"Nice practice-{Guid.NewGuid()}").Bl;
        var nicePracticeEvent = TestData.Events.CreateOccurrenceEvent(userId: OperatorUserId, eventTypeId: nicePractice.Id, isDeleted: true);

        var command    = new SoftDeleteLongEntityCommand<OccurrenceEvent>(Id: nicePracticeEvent.Id, SaveChanges: true, Context: OperatorContext);
        var cqrsResult = await Mediator.Send(request: command);

        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.EntityIsDeleted);
    }

    private static void CheckSoftDeletedEntity<TEntity>(long                 entityId,
                                                        CqrsResult<TEntity?> cqrsResult,
                                                        IQueryable<TEntity>  queryable)
        where TEntity : HrimEntity<long> {
        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.Ok);
        var restored = cqrsResult.Result;
        restored.Should().NotBeNull();
        restored!.Id.Should().Be(expected: entityId);
        restored.IsDeleted.Should().BeTrue();
        restored.ConcurrentToken.Should().Be(expected: 2);
        var entity = queryable.First(x => x.Id == entityId);
        entity.IsDeleted.Should().BeTrue();
        entity.ConcurrentToken.Should().Be(expected: 2);
    }
}