using System.Diagnostics.CodeAnalysis;
using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.EfCore;
using Hrim.Event.Analytics.EfCore.DbEntities.Events;
using Hrimsoft.Core.Extensions;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure;

[ExcludeFromCodeCoverage]
public class TestData {
    private readonly EventAnalyticDbContext _context;

    public TestData(EventAnalyticDbContext context) {
        _context = context;
    }

    public HrimUser CreateUser(Guid id, bool isDeleted = false) {
        lock (_context) {
            var existed = _context.HrimUsers.FirstOrDefault(x => x.Id == id);
            if (existed != null) {
                if (existed.IsDeleted != isDeleted) {
                    existed.IsDeleted = isDeleted;
                    _context.SaveChanges();
                }
                return existed;
            }
            var user = new HrimUser {
                Id              = id,
                CreatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
                ConcurrentToken = 1
            };
            if (isDeleted)
                user.IsDeleted = true;
            _context.HrimUsers.Add(user);
            _context.SaveChanges();
            return user;
        }
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

    public DbDurationEvent CreateDurationEvent(Guid userId, Guid eventTypeId, bool isDeleted = false) {
        var entity = new DbDurationEvent {
            CreatedById     = userId,
            EventTypeId     = eventTypeId,
            StartedOn       = new DateOnly(2020, 09, 1),
            StartedAt       = new DateTimeOffset(2020, 09, 1, 15, 0, 0, TimeSpan.Zero),
            FinishedOn      = new DateOnly(2020, 09, 1),
            FinishedAt      = new DateTimeOffset(2020, 09, 1, 16, 0, 0, TimeSpan.Zero),
            CreatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
            ConcurrentToken = 1
        };
        if (isDeleted)
            entity.IsDeleted = true;
        _context.DurationEvents.Add(entity);
        _context.SaveChanges();
        return entity;
    }

    public DbOccurrenceEvent CreateOccurrenceEvent(Guid userId, Guid eventTypeId, bool isDeleted = false) {
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