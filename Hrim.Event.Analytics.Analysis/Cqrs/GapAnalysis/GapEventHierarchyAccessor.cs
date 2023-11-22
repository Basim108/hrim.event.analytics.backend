using Hrim.Event.Analytics.Analysis.Models;
using Hrim.Event.Analytics.EfCore;
using Hrim.Event.Analytics.EfCore.DbEntities.Events;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.Analysis.Cqrs.GapAnalysis;

/// <summary>
/// Required to mock hierarchical queries. They are not supported by InMemory DB provider
/// </summary>
public interface IGapEventHierarchyAccessor
{
    Task<DateTime?> GetLastUpdatedEventTimeAsync<TEvent>(LTree eventTypePath, CancellationToken cancellationToken)
        where TEvent : DbBaseEvent;

    Task<List<AnalysisEvent>> GetDescendantOccurrencesAsync(LTree eventTypePath, CancellationToken cancellationToken);

    Task<List<AnalysisEvent>> GetDescendantDurationsAsync(LTree eventTypePath, CancellationToken cancellationToken);
}

public class GapEventHierarchyAccessor: IGapEventHierarchyAccessor
{
    private readonly EventAnalyticDbContext _context;

    public GapEventHierarchyAccessor(EventAnalyticDbContext context) { _context = context; }

    public Task<DateTime?> GetLastUpdatedEventTimeAsync<TEvent>(LTree eventTypePath, CancellationToken cancellationToken)
        where TEvent : DbBaseEvent => _context.Set<TEvent>()
                                              .Include(x => x.EventType)
                                              .Where(x => x.EventType!.TreeNodePath.IsDescendantOf(eventTypePath))
                                              .OrderByDescending(x => x.UpdatedAt)
                                              .Select(x => x.UpdatedAt)
                                              .FirstOrDefaultAsync(cancellationToken);

    public Task<List<AnalysisEvent>> GetDescendantOccurrencesAsync(LTree eventTypePath, CancellationToken cancellationToken)
        => _context.OccurrenceEvents
                   .Include(x => x.EventType)
                   .Where(x => x.EventType!.TreeNodePath.IsDescendantOf(eventTypePath) && x.IsDeleted != true)
                   .Select(x => new AnalysisEvent(x.OccurredOn,
                                                  x.OccurredAt,
                                                  null,
                                                  null))
                   .ToListAsync(cancellationToken);

    public Task<List<AnalysisEvent>> GetDescendantDurationsAsync(LTree eventTypePath, CancellationToken cancellationToken)
        => _context.DurationEvents
                   .Include(x => x.EventType)
                   .Where(x => x.EventType!.TreeNodePath.IsDescendantOf(eventTypePath) && x.IsDeleted != true)
                   .Select(x => new AnalysisEvent(x.StartedOn,
                                                  x.StartedAt,
                                                  x.FinishedOn,
                                                  x.FinishedAt))
                   .ToListAsync(cancellationToken);
}