using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.EfCore;
using Hrim.Event.Analytics.EfCore.DbEntities;
using Hrim.Event.Analytics.EfCore.DbEntities.Events;
using Hrimsoft.Core.Extensions;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure;

[ExcludeFromCodeCoverage]
public class EventsData
{
    private readonly EventAnalyticDbContext _context;
    private readonly IMapper                _mapper;

    public EventsData(EventAnalyticDbContext context, IMapper mapper) {
        _context = context;
        _mapper  = mapper;
    }

    public (DbEventType Db, EventType Bl) CreateEventType(long   userId,
                                     string name      = "Test Event Type",
                                     long?  parentId  = null,
                                     bool?  isDeleted = false) {
        var entity = new DbEventType {
            Name            = name,
            Color           = "#ff00cc",
            ParentId        = parentId,
            IsPublic        = false,
            CreatedById     = userId,
            CreatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
            UpdatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
            ConcurrentToken = 1
        };
        if (isDeleted == true)
            entity.IsDeleted = true;
        _context.EventTypes.Add(entity: entity);
        entity.GeneratePath();
        _context.SaveChanges();
        return (entity, _mapper.Map<EventType>(entity));
    }

    public DbDurationEvent CreateDurationEvent(long            userId,
                                               long?           eventTypeId = null,
                                               bool            isDeleted   = false,
                                               DateTimeOffset? startedAt   = null,
                                               DateTimeOffset? finishedAt  = null) {
        eventTypeId ??= CreateEventType(userId, $"event type name: {Guid.NewGuid()}").Bl.Id;
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
            UpdatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
            ConcurrentToken = 1
        };
        if (isDeleted)
            entity.IsDeleted = true;
        if (startedAt.HasValue) {
            entity.StartedAt  = startedAt.Value;
            entity.StartedOn  = new DateOnly(year: startedAt.Value.Year, month: startedAt.Value.Month, day: startedAt.Value.Day);
            entity.FinishedAt = null;
            entity.FinishedOn = null;
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
                                                          long     userId,
                                                          DateOnly start,
                                                          DateOnly end,
                                                          long?    eventTypeId = null) {
        if (end < start)
            throw new ArgumentException($"{nameof(end)}({end}) must be greater or equal to {nameof(start)}({start})",
                                        nameof(end));
        eventTypeId ??= CreateEventType(userId, $"event-type-name: {Guid.NewGuid()}").Bl.Id;
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
                                                              long     userId,
                                                              DateOnly start,
                                                              DateOnly end,
                                                              long?    eventTypeId = null) {
        if (end < start)
            throw new ArgumentException($"{nameof(end)}({end}) must be greater or equal to {nameof(start)}({start})",
                                        nameof(end));
        eventTypeId ??= CreateEventType(userId: userId, $"event-type-name: {Guid.NewGuid()}").Bl.Id;
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
            var @event = CreateOccurrenceEvent(userId,
                                               eventTypeId.Value,
                                               isDeleted: false,
                                               occurredAt: currentStart);
            resultList.Add(item: @event);
        }

        return resultList;
    }

    public DbOccurrenceEvent CreateOccurrenceEvent(long            userId,
                                                   long?           eventTypeId = null,
                                                   bool            isDeleted   = false,
                                                   DateTimeOffset? occurredAt  = null) {
        eventTypeId ??= CreateEventType(userId: userId, $"event-type-name: {Guid.NewGuid()}").Bl.Id;
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
            UpdatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
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
    public Dictionary<long, EventType> CreateManyEventTypes(int count, long userId, bool isDeleted = false) {
        var result = new Dictionary<long, EventType>(capacity: count);
        for (var i = 0; i < count; i++) {
            var db = new DbEventType {
                Name            = $"event type {i}",
                Color           = "#f0c",
                IsPublic        = true,
                CreatedById     = userId,
                CreatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
                UpdatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
                ConcurrentToken = 1
            };
            if (isDeleted)
                db.IsDeleted = true;
            _context.EventTypes.Add(entity: db);
            var bl = _mapper.Map<EventType>(db);
            result.Add(key: db.Id, value: bl);
        }
        _context.SaveChanges();
        return result;
    }
}