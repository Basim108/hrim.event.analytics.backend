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

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Entity;

[ExcludeFromCodeCoverage]
public class SoftDeleteEntityTests: BaseCqrsTests {
    private readonly IMapper _mapper;

    public SoftDeleteEntityTests() {
        _mapper = ServiceProvider.GetRequiredService<IMapper>();
    }

    [Fact]
    public async Task EventType_Should_Forbid() {
        var anotherId = Guid.NewGuid();
        TestData.Users.EnsureUserExistence(anotherId);
        var headache = TestData.Events.CreateEventType(anotherId, $"Headache-{Guid.NewGuid()}");

        var command    = new SoftDeleteEntityCommand<UserEventType>(headache.Id, SaveChanges: true, OperatorContext);
        var cqrsResult = await Mediator.Send(command);

        cqrsResult.StatusCode.Should().Be(CqrsResultCode.Forbidden);
    }

    [Fact]
    public async Task DurationEvent_Should_Forbid() {
        var anotherId = Guid.NewGuid();
        TestData.Users.EnsureUserExistence(anotherId);
        var headache      = TestData.Events.CreateEventType(anotherId, $"Headache-{Guid.NewGuid()}");
        var durationEvent = TestData.Events.CreateDurationEvent(anotherId, headache.Id);

        var command    = new SoftDeleteEntityCommand<DurationEvent>(durationEvent.Id, SaveChanges: true, OperatorContext);
        var cqrsResult = await Mediator.Send(command);

        cqrsResult.StatusCode.Should().Be(CqrsResultCode.Forbidden);
    }

    [Fact]
    public async Task OccurrenceEvent_Should_Forbid() {
        var anotherId = Guid.NewGuid();
        TestData.Users.EnsureUserExistence(anotherId);
        var nicePractice    = TestData.Events.CreateEventType(anotherId, $"Nice Practice-{Guid.NewGuid()}");
        var occurrenceEvent = TestData.Events.CreateOccurrenceEvent(anotherId, nicePractice.Id);

        var command    = new SoftDeleteEntityCommand<OccurrenceEvent>(occurrenceEvent.Id, SaveChanges: true, OperatorContext);
        var cqrsResult = await Mediator.Send(command);

        cqrsResult.StatusCode.Should().Be(CqrsResultCode.Forbidden);
    }
    
    [Fact]
    public async Task User_NotFound() {
        var entityId = Guid.NewGuid();
        var command    = new SoftDeleteEntityCommand<HrimUser>(entityId, SaveChanges: true, OperatorContext);
        var cqrsResult = await Mediator.Send(command);

        cqrsResult.StatusCode.Should().Be(CqrsResultCode.NotFound);
    }
    
    [Fact]
    public async Task EventType_NotFound() {
        var entityId   = Guid.NewGuid();
        var command    = new SoftDeleteEntityCommand<UserEventType>(entityId, SaveChanges: true, OperatorContext);
        var cqrsResult = await Mediator.Send(command);

        cqrsResult.StatusCode.Should().Be(CqrsResultCode.NotFound);
    }
    
    [Fact]
    public async Task DurationEvent_NotFound() {
        var entityId   = Guid.NewGuid();
        var command    = new SoftDeleteEntityCommand<DurationEvent>(entityId, SaveChanges: true, OperatorContext);
        var cqrsResult = await Mediator.Send(command);

        cqrsResult.StatusCode.Should().Be(CqrsResultCode.NotFound);
    }
    
    [Fact]
    public async Task OccurrenceEvent_NotFound() {
        var entityId   = Guid.NewGuid();
        var command    = new SoftDeleteEntityCommand<OccurrenceEvent>(entityId, SaveChanges: true, OperatorContext);
        var cqrsResult = await Mediator.Send(command);

        cqrsResult.StatusCode.Should().Be(CqrsResultCode.NotFound);
    }
    
    [Fact]
    public async Task Tag_NotFound() {
        var entityId   = Guid.NewGuid();
        var command    = new SoftDeleteEntityCommand<HrimTag>(entityId, SaveChanges: true, OperatorContext);
        var cqrsResult = await Mediator.Send(command);

        cqrsResult.StatusCode.Should().Be(CqrsResultCode.NotFound);
    }
    
    [Fact]
    public async Task User_Should_SoftDelete() {
        var entityId = Guid.NewGuid();
        TestData.Users.EnsureUserExistence(entityId, isDeleted: false);
        var command    = new SoftDeleteEntityCommand<HrimUser>(entityId, SaveChanges: true, OperatorContext);
        var cqrsResult = await Mediator.Send(command);

        CheckSoftDeletedEntity(entityId, cqrsResult, TestData.DbContext.HrimUsers);
    }

    [Fact]
    public async Task EventType_Should_SoftDelete() {
        var headache = TestData.Events.CreateEventType(OperatorContext.UserId, $"Headache-{Guid.NewGuid()}");

        var command    = new SoftDeleteEntityCommand<UserEventType>(headache.Id, SaveChanges: true, OperatorContext);
        var cqrsResult = await Mediator.Send(command);

        CheckSoftDeletedEntity(headache.Id, cqrsResult, TestData.DbContext.UserEventTypes);
    }

    [Fact]
    public async Task DurationEvent_Should_SoftDelete() {
        var headache      = TestData.Events.CreateEventType(OperatorContext.UserId, $"Headache-{Guid.NewGuid()}");
        var durationEvent = TestData.Events.CreateDurationEvent(OperatorContext.UserId, headache.Id);

        var command    = new SoftDeleteEntityCommand<DurationEvent>(durationEvent.Id, SaveChanges: true, OperatorContext);
        var cqrsResult = await Mediator.Send(command);

        var dbEntity     = _mapper.Map<DbDurationEvent>(cqrsResult.Result);
        var dbCqrsResult = new CqrsResult<DbDurationEvent?>(dbEntity, cqrsResult.StatusCode, cqrsResult.Info);
        CheckSoftDeletedEntity(durationEvent.Id, dbCqrsResult, TestData.DbContext.DurationEvents);
    }

    [Fact]
    public async Task OccurrenceEvent_Should_SoftDelete() {
        var nicePractice      = TestData.Events.CreateEventType(OperatorContext.UserId, $"Nice practice-{Guid.NewGuid()}");
        var nicePracticeEvent = TestData.Events.CreateOccurrenceEvent(OperatorContext.UserId, nicePractice.Id);

        var command      = new SoftDeleteEntityCommand<OccurrenceEvent>(nicePracticeEvent.Id, SaveChanges: true, OperatorContext);
        var cqrsResult   = await Mediator.Send(command);
        
        var dbEntity     = _mapper.Map<DbOccurrenceEvent>(cqrsResult.Result);
        var dbCqrsResult = new CqrsResult<DbOccurrenceEvent?>(dbEntity, cqrsResult.StatusCode, cqrsResult.Info);
        CheckSoftDeletedEntity(nicePracticeEvent.Id, dbCqrsResult, TestData.DbContext.OccurrenceEvents);
    }

    [Fact]
    public async Task Deleted_User_Should_Recognize() {
        var entityId = Guid.NewGuid();
        TestData.Users.EnsureUserExistence(entityId, isDeleted: true);
        var command    = new SoftDeleteEntityCommand<HrimUser>(entityId, SaveChanges: true, OperatorContext);
        var cqrsResult = await Mediator.Send(command);

        cqrsResult.StatusCode.Should().Be(CqrsResultCode.EntityIsDeleted);
    }

    [Fact]
    public async Task Deleted_EventType_Should_Recognize() {
        var headache = TestData.Events.CreateEventType(OperatorContext.UserId, $"Headache-{Guid.NewGuid()}", isDeleted: true);

        var command    = new SoftDeleteEntityCommand<UserEventType>(headache.Id, SaveChanges: true, OperatorContext);
        var cqrsResult = await Mediator.Send(command);

        cqrsResult.StatusCode.Should().Be(CqrsResultCode.EntityIsDeleted);
    }

    [Fact]
    public async Task Deleted_DurationEvent_Should_Recognize() {
        var headache      = TestData.Events.CreateEventType(OperatorContext.UserId, $"Headache-{Guid.NewGuid()}");
        var durationEvent = TestData.Events.CreateDurationEvent(OperatorContext.UserId, headache.Id, isDeleted: true);

        var command    = new SoftDeleteEntityCommand<DurationEvent>(durationEvent.Id, SaveChanges: true, OperatorContext);
        var cqrsResult = await Mediator.Send(command);

        cqrsResult.StatusCode.Should().Be(CqrsResultCode.EntityIsDeleted);
    }

    [Fact]
    public async Task Deleted_OccurrenceEvent_Should_Recognize() {
        var nicePractice      = TestData.Events.CreateEventType(OperatorContext.UserId, $"Nice practice-{Guid.NewGuid()}");
        var nicePracticeEvent = TestData.Events.CreateOccurrenceEvent(OperatorContext.UserId, nicePractice.Id, isDeleted: true);

        var command    = new SoftDeleteEntityCommand<OccurrenceEvent>(nicePracticeEvent.Id, SaveChanges: true, OperatorContext);
        var cqrsResult = await Mediator.Send(command);

        cqrsResult.StatusCode.Should().Be(CqrsResultCode.EntityIsDeleted);
    }
    
    private static void CheckSoftDeletedEntity<TEntity>(Guid entityId, CqrsResult<TEntity?> cqrsResult, IQueryable<TEntity> queryable)
        where TEntity : HrimEntity {
        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(CqrsResultCode.Ok);
        var restored = cqrsResult.Result;
        restored.Should().NotBeNull();
        restored!.Id.Should().Be(entityId);
        restored.IsDeleted.Should().BeTrue();
        restored.ConcurrentToken.Should().Be(2);
        var entity = queryable.First(x => x.Id == entityId);
        entity.IsDeleted.Should().BeTrue();
        entity.ConcurrentToken.Should().Be(2);
    }
}