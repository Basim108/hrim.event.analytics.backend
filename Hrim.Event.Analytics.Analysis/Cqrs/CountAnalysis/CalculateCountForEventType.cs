using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.Analysis.Cqrs.CountAnalysis.Models;
using Hrim.Event.Analytics.Analysis.Models;
using Hrim.Event.Analytics.EfCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.Analysis.Cqrs.CountAnalysis;

/// <summary>
/// Calculates number of events and info about duration events (total duration, min, max and avg duration) of a specific event-type 
/// </summary>
/// <param name="EventTypeId">Id of an event-type for which events we should perform an analysis</param>
/// <param name="LastRun">Previous analysis result.</param>
/// <returns> Returns analysis result or null if there was no changes after the last run.</returns>
public record CalculateCountForEventType(Guid EventTypeId, StatisticsForEventType? LastRun)
    : IRequest<CountAnalysisResult?>;

public class CalculateCountForEventTypeHandler: IRequestHandler<CalculateCountForEventType, CountAnalysisResult?>
{
    private readonly ILogger<CalculateCountForEventTypeHandler> _logger;
    private readonly ICountCalculationService                   _calcService;
    private readonly EventAnalyticDbContext                     _context;

    public CalculateCountForEventTypeHandler(ILogger<CalculateCountForEventTypeHandler> logger,
                                             ICountCalculationService                   calcService,
                                             EventAnalyticDbContext                     context) {
        _logger      = logger;
        _calcService = calcService;
        _context     = context;
    }

    public Task<CountAnalysisResult?> Handle(CalculateCountForEventType request, CancellationToken cancellationToken) {
        if (request.EventTypeId == Guid.Empty)
            throw new ArgumentNullException(nameof(request), nameof(request.EventTypeId));
        // CASE 1: there is no events (all deleted after last run) => remove pre analysis result from db  => then result.EventCount = 0
        // CASE 2: there is no events and no last run  => do nothing even analysis_result should be null
        // CASE 3: there is no changes before last run and there are changes after last run => recalculate everything
        // CASE 4: there is changes before last run  => recalculate everything
        // CASE 5: there is no changes neither before nor after last run => no calculation required
        // CASE 6: Ignores deleted events
        return CalculateAsync(request, cancellationToken);
    }

    private async Task<CountAnalysisResult?> CalculateAsync(CalculateCountForEventType request, CancellationToken cancellationToken) {
        var lastUpdatedDuration = await _context.DurationEvents
                                                .Where(x => x.EventTypeId == request.EventTypeId)
                                                .OrderByDescending(x => x.UpdatedAt)
                                                .Select(x => x.UpdatedAt)
                                                .FirstOrDefaultAsync(cancellationToken);
        var lastUpdatedOccurrence = await _context.OccurrenceEvents
                                                  .Where(x => x.EventTypeId == request.EventTypeId)
                                                  .OrderByDescending(x => x.UpdatedAt)
                                                  .Select(x => x.UpdatedAt)
                                                  .FirstOrDefaultAsync(cancellationToken);
        var isFirstRun          = request.LastRun == null;
        var isDurationChanged   = lastUpdatedDuration.HasValue   && isFirstRun || lastUpdatedDuration.HasValue   && lastUpdatedDuration   > request.LastRun!.StartedAt;
        var isOccurrenceChanged = lastUpdatedOccurrence.HasValue && isFirstRun || lastUpdatedOccurrence.HasValue && lastUpdatedOccurrence > request.LastRun!.StartedAt;

        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug(AnalysisLogs.GAP_CALCULATION_PARAMS, isFirstRun, isDurationChanged, isOccurrenceChanged);

        if (!isDurationChanged && !isOccurrenceChanged)
            return null; // CASE 2, 5

        var occurrences = await _context.OccurrenceEvents
                                        .Where(x => x.EventTypeId == request.EventTypeId && x.IsDeleted != true)
                                        .CountAsync(cancellationToken);
        var durations = await _context.DurationEvents
                                      .Where(x => x.EventTypeId == request.EventTypeId && x.IsDeleted != true)
                                      .OrderBy(x => x.StartedOn)
                                      .ThenBy(x => x.StartedAt)
                                      .Select(x => new AnalysisEvent(x.StartedOn,
                                                                     x.StartedAt,
                                                                     x.FinishedOn,
                                                                     x.FinishedAt))
                                      .ToListAsync(cancellationToken);
        // CASE 1, 3, 4, 6
        return _calcService.Calculate(durations, occurrences);
    }
}