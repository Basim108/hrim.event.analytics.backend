using Hrim.Event.Analytics.Analysis.Models;
using Hrim.Event.Analytics.EfCore;
using Hrim.Event.Analytics.EfCore.DbEntities.Events;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.Analysis.Cqrs.CountAnalysis;

/// <summary>
/// Required to mock hierarchical queries. They are not supported by InMemory DB provider
/// </summary>
public interface ICountEventHierarchyAccessor
{
    Task<DateTime?> GetLastUpdatedEventTimeAsync<TEvent>(LTree eventTypePath, CancellationToken cancellationToken)
        where TEvent : DbBaseEvent;

    Task<int> CountDescendantOccurrencesAsync(LTree eventTypePath, CancellationToken cancellationToken);

    Task<List<AnalysisEvent>> GetDescendantDurationsAsync(LTree eventTypePath, CancellationToken cancellationToken);
}

public class CountEventHierarchyAccessor: ICountEventHierarchyAccessor
{
    private readonly EventAnalyticDbContext _context;

    public CountEventHierarchyAccessor(EventAnalyticDbContext context) { _context = context; }

    public Task<DateTime?> GetLastUpdatedEventTimeAsync<TEvent>(LTree eventTypePath, CancellationToken cancellationToken)
        where TEvent : DbBaseEvent => _context.Set<TEvent>()
                                              .Include(x => x.EventType)
                                              .Where(x => x.EventType!.TreeNodePath!.Value.IsDescendantOf(eventTypePath))
                                              .OrderByDescending(x => x.UpdatedAt)
                                              .Select(x => x.UpdatedAt)
                                              .FirstOrDefaultAsync(cancellationToken);

    public Task<int> CountDescendantOccurrencesAsync(LTree eventTypePath, CancellationToken cancellationToken)
        => _context.OccurrenceEvents
                   .Include(x => x.EventType)
                   .Where(x => x.EventType!.TreeNodePath!.Value.IsDescendantOf(eventTypePath)
                            && x.IsDeleted != true)
                   .CountAsync(cancellationToken);

    public Task<List<AnalysisEvent>> GetDescendantDurationsAsync(LTree eventTypePath, CancellationToken cancellationToken)
        => _context.DurationEvents
                   .Include(x => x.EventType)
                   .Where(x => x.EventType!.TreeNodePath!.Value.IsDescendantOf(eventTypePath)
                            && x.IsDeleted != true)
                   .OrderBy(x => x.StartedOn)
                   .ThenBy(x => x.StartedAt)
                   .Select(x => new AnalysisEvent(x.StartedOn,
                                                  x.StartedAt,
                                                  x.FinishedOn,
                                                  x.FinishedAt))
                   .ToListAsync(cancellationToken);
}