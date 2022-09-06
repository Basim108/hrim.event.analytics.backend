using System.Diagnostics.CodeAnalysis;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.EfCore;
using Hrim.Event.Analytics.EfCore.DbEntities.Events;
using Hrimsoft.Core.Extensions;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure;

[ExcludeFromCodeCoverage]
public class EventsData {
    private readonly EventAnalyticDbContext _context;

    public EventsData(EventAnalyticDbContext context) {
        _context = context;
    }

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
        _context.UserEventTypes.Add(entity);
        _context.SaveChanges();
        return entity;
    }

    public DbDurationEvent CreateDurationEvent(Guid            userId,
                                               Guid            eventTypeId,
                                               bool            isDeleted  = false,
                                               DateTimeOffset? startedAt  = null,
                                               DateTimeOffset? finishedAt = null) {
        var entity = new DbDurationEvent {
            CreatedById     = userId,
            EventTypeId     = eventTypeId,
            StartedOn       = new DateOnly(2020, 09, 1),
            StartedAt       = new DateTimeOffset(2020, 09, 1, 15, 0, 0, TimeSpan.FromHours(4)),
            FinishedOn      = new DateOnly(2020, 09, 1),
            FinishedAt      = new DateTimeOffset(2020, 09, 1, 16, 0, 0, TimeSpan.FromHours(4)),
            CreatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
            ConcurrentToken = 1
        };
        if (isDeleted)
            entity.IsDeleted = true;
        if (startedAt.HasValue) {
            entity.StartedAt = startedAt.Value;
            entity.StartedOn = new DateOnly(startedAt.Value.Year, startedAt.Value.Month, startedAt.Value.Day);
        }
        if (finishedAt.HasValue) {
            entity.FinishedAt = finishedAt.Value;
            entity.FinishedOn = new DateOnly(finishedAt.Value.Year, finishedAt.Value.Month, finishedAt.Value.Day);
        }
        _context.DurationEvents.Add(entity);
        _context.SaveChanges();
        return entity;
    }

    public DbOccurrenceEvent CreateOccurrenceEvent(Guid            userId,
                                                   Guid            eventTypeId,
                                                   bool            isDeleted  = false,
                                                   DateTimeOffset? occurredAt = null) {
        var entity = new DbOccurrenceEvent() {
            CreatedById     = userId,
            EventTypeId     = eventTypeId,
            OccurredOn      = new DateOnly(2020, 04, 12),
            OccurredAt      = new DateTimeOffset(2020, 04, 12, 23, 0, 0, TimeSpan.Zero),
            CreatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
            ConcurrentToken = 1
        };
        if (isDeleted)
            entity.IsDeleted = true;
        if (occurredAt.HasValue) {
            entity.OccurredAt = occurredAt.Value.TruncateToMilliseconds();
            entity.OccurredOn = new DateOnly(occurredAt.Value.Year,
                                             occurredAt.Value.Month,
                                             occurredAt.Value.Day);
        }
        _context.OccurrenceEvents.Add(entity);
        _context.SaveChanges();
        return entity;
    }

    /// <summary> Creates number of event types </summary>
    /// <param name="count">the number of entities that has to be created</param>
    /// <param name="userId">an owner</param>
    /// <param name="isDeleted">should they be deleted</param>
    public Dictionary<Guid, UserEventType> CreateManyEventTypes(int count, Guid userId, bool isDeleted = false) {
        var result = new Dictionary<Guid, UserEventType>(count);
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
            _context.UserEventTypes.Add(entity);
            _context.SaveChanges();
            result.Add(entity.Id, entity);
        }
        return result;
    }
}