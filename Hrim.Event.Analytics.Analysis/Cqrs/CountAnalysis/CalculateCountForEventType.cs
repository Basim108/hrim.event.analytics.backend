using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.Analysis.Cqrs.CountAnalysis.Models;
using Hrim.Event.Analytics.EfCore.DbEntities.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.Analysis.Cqrs.CountAnalysis;

/// <summary>
/// Calculates number of events and info about duration events (total duration, min, max and avg duration) of a specific event-type 
/// </summary>
/// <param name="EventTypeInfo">An event-type for which events we should perform an analysis</param>
/// <param name="LastRun">Previous analysis result.</param>
/// <returns> Returns analysis result or null if there was no changes after the last run.</returns>
public record CalculateCountForEventType(EventTypeAnalysisSettings EventTypeInfo, StatisticsForEventType? LastRun)
    : IRequest<CountAnalysisResult?>;

public class CalculateCountForEventTypeHandler: IRequestHandler<CalculateCountForEventType, CountAnalysisResult?>
{
    private readonly ILogger<CalculateCountForEventTypeHandler> _logger;
    private readonly ICountCalculationService                   _calcService;
    private readonly ICountEventHierarchyAccessor               _hierarchyAccessor;

    public CalculateCountForEventTypeHandler(ILogger<CalculateCountForEventTypeHandler> logger,
                                             ICountCalculationService                   calcService,
                                             ICountEventHierarchyAccessor               hierarchyAccessor) {
        _logger            = logger;
        _calcService       = calcService;
        _hierarchyAccessor = hierarchyAccessor;
    }

    public Task<CountAnalysisResult?> Handle(CalculateCountForEventType request, CancellationToken cancellationToken) {
        if (request.EventTypeInfo.EventTypeId == default)
            throw new ArgumentNullException(nameof(request), nameof(request.EventTypeInfo.EventTypeId));
        // CASE 1: there are no events (all deleted after last run) => remove pre analysis result from db  => then result.EventCount = 0
        // CASE 2: there are no events and no last run  => do nothing even analysis_result should be null
        // CASE 3: there are no changes before last run and there are changes after last run => recalculate everything
        // CASE 4: there are changes before last run  => recalculate everything
        // CASE 5: there are no changes neither before nor after last run => no calculation required
        // CASE 6: After last run having a new soft deleted event should ignore it and no calculation required
        return CalculateAsync(request, cancellationToken);
    }

    private async Task<CountAnalysisResult?> CalculateAsync(CalculateCountForEventType request, CancellationToken cancellationToken) {
        var lastUpdatedDuration = await _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbDurationEvent>(request.EventTypeInfo.TreeNodePath,
                                                                                                         cancellationToken);
        var lastUpdatedOccurrence = await _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbOccurrenceEvent>(request.EventTypeInfo.TreeNodePath,
                                                                                                             cancellationToken);
        var isFirstRun          = request.LastRun == null;
        var isDurationChanged   = lastUpdatedDuration.HasValue   && isFirstRun || 
                                  lastUpdatedDuration.HasValue   && lastUpdatedDuration   > request.LastRun!.StartedAt;
        var isOccurrenceChanged = lastUpdatedOccurrence.HasValue && isFirstRun || 
                                  lastUpdatedOccurrence.HasValue && lastUpdatedOccurrence > request.LastRun!.StartedAt;

        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug(AnalysisLogs.GAP_CALCULATION_PARAMS, isFirstRun, isDurationChanged, isOccurrenceChanged, false);

        if (!isDurationChanged && !isOccurrenceChanged)
            return null; // CASE 2, 5

        var occurrences = await _hierarchyAccessor.CountDescendantOccurrencesAsync(request.EventTypeInfo.TreeNodePath,
                                                                                   cancellationToken);
        var durations = await _hierarchyAccessor.GetDescendantDurationsAsync(request.EventTypeInfo.TreeNodePath,
                                                                             cancellationToken);
        // CASE 1, 3, 4, 6
        return _calcService.Calculate(durations, occurrences);
    }
}