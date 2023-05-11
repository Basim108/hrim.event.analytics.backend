using System.Diagnostics.CodeAnalysis;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.EfCore;
using Hrim.Event.Analytics.EfCore.DbEntities.Events;
using Hrimsoft.Core.Extensions;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure;

[ExcludeFromCodeCoverage]
public class EventsData
{
    private readonly EventAnalyticDbContext _context;

    public EventsData(EventAnalyticDbContext context) { _context = context; }

    public UserEventType CreateEventType(Guid userId, string name, bool isDeleted = false) {
        var entity = new UserEventType {
            Name            = name,
            Color           = "#ff00cc",
            IsPublic        = true,
            CreatedById     = userId,
            CreatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
            ConcurrentToken = 1
        };
        if (isDeleted)
            entity.IsDeleted = true;
        _context.UserEventTypes.Add(entity: entity);
        _context.SaveChanges();
        return entity;
    }

    public DbDurationEvent CreateDurationEvent(Guid            userId,
                                               Guid?           eventTypeId = null,
                                               bool            isDeleted   = false,
                                               DateTimeOffset? startedAt   = null,
                                               DateTimeOffset? finishedAt  = null) {
        eventTypeId ??= CreateEventType(userId: userId, $"event type name: {Guid.NewGuid()}").Id;
        var entity = new DbDurationEvent {
            CreatedById = userId,
            EventTypeId = eventTypeId.Value,
            StartedOn   = new DateOnly(year: 2020, month: 09, day: 1),
            StartedAt = new DateTimeOffset(year: 2020,
                                           month: 09,
                                           day: 1,
                                           hour: 15,
                                           minute: 0,
                                           second: 0,
                                           TimeSpan.FromHours(value: 4)),
            FinishedOn = new DateOnly(year: 2020, month: 09, day: 1),
            FinishedAt = new DateTimeOffset(year: 2020,
                                            month: 09,
                                            day: 1,
                                            hour: 16,
                                            minute: 0,
                                            second: 0,
                                            TimeSpan.FromHours(value: 4)),
            CreatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
            ConcurrentToken = 1
        };
        if (isDeleted)
            entity.IsDeleted = true;
        if (startedAt.HasValue) {
            entity.StartedAt = startedAt.Value;
            entity.StartedOn = new DateOnly(year: startedAt.Value.Year, month: startedAt.Value.Month, day: startedAt.Value.Day);
        }

        if (finishedAt.HasValue) {
            entity.FinishedAt = finishedAt.Value;
            entity.FinishedOn = new DateOnly(year: finishedAt.Value.Year, month: finishedAt.Value.Month, day: finishedAt.Value.Day);
        }

        _context.DurationEvents.Add(entity: entity);
        _context.SaveChanges();
        return entity;
    }

    public List<DbDurationEvent> CreateManyDurationEvents(int      count,
                                                          Guid     userId,
                                                          DateOnly start,
                                                          DateOnly end,
                                                          Guid?    eventTypeId = null) {
        if (end < start)
            throw new ArgumentException($"{nameof(end)}({end}) must be greater or equal to {nameof(start)}({start})",
                                        nameof(end));
        eventTypeId ??= CreateEventType(userId: userId, $"event-type-name: {Guid.NewGuid()}").Id;
        var startedAt = new DateTimeOffset(year: start.Year,
                                           month: start.Month,
                                           day: start.Day,
                                           hour: 0,
                                           minute: 0,
                                           second: 0,
                                           offset: TimeSpan.Zero);
        var finishedAt = new DateTimeOffset(year: end.Year,
                                            month: end.Month,
                                            day: end.Day,
                                            hour: 23,
                                            minute: 59,
                                            second: 59,
                                            offset: TimeSpan.Zero);
        var timeStep   = (finishedAt - startedAt).TotalHours / count;
        var resultList = new List<DbDurationEvent>(capacity: count);
        for (var i = 0; i < count; i++) {
            var currentStart  = startedAt.AddHours(timeStep * i);
            var currentFinish = currentStart.AddHours(hours: timeStep);
            var @event = CreateDurationEvent(userId: userId,
                                             eventTypeId: eventTypeId.Value,
                                             isDeleted: false,
                                             startedAt: currentStart,
                                             finishedAt: currentFinish);
            resultList.Add(item: @event);
        }

        return resultList;
    }

    public List<DbOccurrenceEvent> CreateManyOccurrenceEvents(int      count,
                                                              Guid     userId,
                                                              DateOnly start,
                                                              DateOnly end,
                                                              Guid?    eventTypeId = null) {
        if (end < start)
            throw new ArgumentException($"{nameof(end)}({end}) must be greater or equal to {nameof(start)}({start})",
                                        nameof(end));
        eventTypeId ??= CreateEventType(userId: userId, $"event-type-name: {Guid.NewGuid()}").Id;
        var startedAt = new DateTimeOffset(year: start.Year,
                                           month: start.Month,
                                           day: start.Day,
                                           hour: 0,
                                           minute: 0,
                                           second: 0,
                                           offset: TimeSpan.Zero);
        var finishedAt = new DateTimeOffset(year: end.Year,
                                            month: end.Month,
                                            day: end.Day,
                                            hour: 23,
                                            minute: 59,
                                            second: 59,
                                            offset: TimeSpan.Zero);
        var timeStep   = (finishedAt - startedAt).TotalHours / count;
        var resultList = new List<DbOccurrenceEvent>(capacity: count);
        for (var i = 0; i < count; i++) {
            var currentStart = startedAt.AddHours(timeStep * i);
            var @event = CreateOccurrenceEvent(userId: userId,
                                               eventTypeId: eventTypeId.Value,
                                               isDeleted: false,
                                               occurredAt: currentStart);
            resultList.Add(item: @event);
        }

        return resultList;
    }

    public DbOccurrenceEvent CreateOccurrenceEvent(Guid            userId,
                                                   Guid?           eventTypeId = null,
                                                   bool            isDeleted   = false,
                                                   DateTimeOffset? occurredAt  = null) {
        eventTypeId ??= CreateEventType(userId: userId, $"event-type-name: {Guid.NewGuid()}").Id;
        var entity = new DbOccurrenceEvent {
            CreatedById = userId,
            EventTypeId = eventTypeId.Value,
            OccurredOn  = new DateOnly(year: 2020, month: 04, day: 12),
            OccurredAt = new DateTimeOffset(year: 2020,
                                            month: 04,
                                            day: 12,
                                            hour: 23,
                                            minute: 0,
                                            second: 0,
                                            offset: TimeSpan.Zero),
            CreatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
            ConcurrentToken = 1
        };
        if (isDeleted)
            entity.IsDeleted = true;
        if (occurredAt.HasValue) {
            entity.OccurredAt = occurredAt.Value.TruncateToMilliseconds();
            entity.OccurredOn = new DateOnly(year: occurredAt.Value.Year,
                                             month: occurredAt.Value.Month,
                                             day: occurredAt.Value.Day);
        }

        _context.OccurrenceEvents.Add(entity: entity);
        _context.SaveChanges();
        return entity;
    }

    /// <summary> Creates number of event types </summary>
    /// <param name="count">the number of entities that has to be created</param>
    /// <param name="userId">an owner</param>
    /// <param name="isDeleted">should they be deleted</param>
    public Dictionary<Guid, UserEventType> CreateManyEventTypes(int count, Guid userId, bool isDeleted = false) {
        var result = new Dictionary<Guid, UserEventType>(capacity: count);
        for (var i = 0; i < count; i++) {
            var entity = new UserEventType {
                Name            = $"event type {i}",
                Color           = "#f0c",
                IsPublic        = true,
                CreatedById     = userId,
                CreatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
                ConcurrentToken = 1
            };
            if (isDeleted)
                entity.IsDeleted = true;
            _context.UserEventTypes.Add(entity: entity);
            _context.SaveChanges();
            result.Add(key: entity.Id, value: entity);
        }

        return result;
    }
}