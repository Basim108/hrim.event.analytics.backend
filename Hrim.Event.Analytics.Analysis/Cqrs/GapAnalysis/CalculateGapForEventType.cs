using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.Abstractions.Extensions;
using Hrim.Event.Analytics.Analysis.Cqrs.GapAnalysis.Models;
using Hrim.Event.Analytics.Analysis.Models;
using Hrim.Event.Analytics.EfCore;
using Hrimsoft.Core.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.Analysis.Cqrs.GapAnalysis;

/// <summary>
/// Calculates gaps between events of a specific event-type 
/// </summary>
/// <param name="CalculationInfo">Id of  an event-type for which events we should calculate gaps</param>
/// <param name="LastRun">Previous analysis result.</param>
/// <returns> Returns analysis result or null if there was no changes after the last run.</returns>
public record CalculateGapForEventType(EventTypeAnalysisSettings CalculationInfo, StatisticsForEventType? LastRun)
    : IRequest<GapAnalysisResult?>;

public class CalculateGapForEventTypeHandler: IRequestHandler<CalculateGapForEventType, GapAnalysisResult?>
{
    private readonly ILogger<CalculateGapForEventTypeHandler> _logger;
    private readonly IGapCalculationService                   _calcService;
    private readonly EventAnalyticDbContext                   _context;

    public CalculateGapForEventTypeHandler(ILogger<CalculateGapForEventTypeHandler> logger,
                                           IGapCalculationService                   calcService,
                                           EventAnalyticDbContext                   context) {
        _logger      = logger;
        _calcService = calcService;
        _context     = context;
    }

    public Task<GapAnalysisResult?> Handle(CalculateGapForEventType request, CancellationToken cancellationToken) {
        if (request.CalculationInfo == null)
            throw new ArgumentNullException(nameof(request), nameof(request.CalculationInfo));
        if(request.CalculationInfo.EventTypeId == default)
            throw new ArgumentNullException(nameof(request), nameof(request.CalculationInfo.EventTypeId));
            
        // CASE 1: there is no events (all deleted after last run) => remove pre analysis result from db  => then result.EventCount = 0
        // CASE 2: there is no events and no last run  => do nothing even analysis_result should be null
        // CASE 3: there is no changes before last run and there are changes after last run => recalculate everything
        // CASE 4: there is changes before last run  => recalculate everything
        // CASE 5: there is no changes neither before nor after last run => no calculation required
        // CASE 6: there are shuffled event kinds: e.g. sequence of occurrence, duration, occurrence
        // CASE 7: Ignores deleted events
        // CASE 8: there is no changes in events, but analysis settings changed after last run => recalculate
        return CalculateAsync(request, cancellationToken);
    }

    private async Task<GapAnalysisResult?> CalculateAsync(CalculateGapForEventType request, CancellationToken cancellationToken) {
        var lastUpdatedDuration = await _context.DurationEvents
                                                .Where(x => x.EventTypeId == request.CalculationInfo.EventTypeId)
                                                .OrderByDescending(x => x.UpdatedAt)
                                                .Select(x => x.UpdatedAt)
                                                .FirstOrDefaultAsync(cancellationToken);
        var lastUpdatedOccurrence = await _context.OccurrenceEvents
                                                  .Where(x => x.EventTypeId == request.CalculationInfo.EventTypeId)
                                                  .OrderByDescending(x => x.UpdatedAt)
                                                  .Select(x => x.UpdatedAt)
                                                  .FirstOrDefaultAsync(cancellationToken);
        var isFirstRun          = request.LastRun == null;
        var isDurationChanged   = lastUpdatedDuration.HasValue   && isFirstRun || 
                                  lastUpdatedDuration.HasValue   && lastUpdatedDuration   > request.LastRun!.StartedAt;
        var isOccurrenceChanged = lastUpdatedOccurrence.HasValue && isFirstRun || 
                                  lastUpdatedOccurrence.HasValue && lastUpdatedOccurrence > request.LastRun!.StartedAt;
        var isCalcSettingsChanged         = isFirstRun || request.CalculationInfo.UpdatedAt > request.LastRun?.StartedAt;
        var recalculateDueToSettingChange = (lastUpdatedDuration.HasValue || lastUpdatedOccurrence.HasValue) && isCalcSettingsChanged;
        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug(AnalysisLogs.GAP_CALCULATION_PARAMS, 
                             isFirstRun, isDurationChanged, isOccurrenceChanged, isCalcSettingsChanged);

        if (!isDurationChanged && !isOccurrenceChanged && !recalculateDueToSettingChange)
            return null; // CASE 2, 5
        
        var occurrences = await _context.OccurrenceEvents
                                        .Where(x => x.EventTypeId == request.CalculationInfo.EventTypeId && 
                                                    x.IsDeleted   != true)
                                        .Select(x => new AnalysisEvent(x.OccurredOn, x.OccurredAt,
                                                                       null, null))
                                        .ToListAsync(cancellationToken);
        // add last occurrence in order to calculate the gap between the last event and now
        var now = DateTimeOffset.UtcNow.TruncateToSeconds();
        occurrences.Add(new AnalysisEvent(now.ToDateOnly(), now, null, null));
        
        var durations = await _context.DurationEvents
                                      .Where(x => x.EventTypeId == request.CalculationInfo.EventTypeId && 
                                                  x.IsDeleted   != true)
                                      .Select(x => new AnalysisEvent(x.StartedOn,  x.StartedAt,
                                                                     x.FinishedOn, x.FinishedAt))
                                      .ToListAsync(cancellationToken);
        var joinedEvents = durations.Concat(occurrences)
                                    .OrderBy(x => x.StartDate)
                                    .ThenBy(x => x.StartTime)
                                    .ToList();
        
        // CASE 1, 3, 4, 6, 7
        var gapSettings = new GapSettings(request.CalculationInfo.Settings!);
        return _calcService.Calculate(joinedEvents, gapSettings);
    }
}