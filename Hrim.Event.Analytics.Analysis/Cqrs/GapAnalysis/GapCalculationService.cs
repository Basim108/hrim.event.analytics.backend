using Hrim.Event.Analytics.Abstractions.Extensions;
using Hrim.Event.Analytics.Analysis.Cqrs.GapAnalysis.Models;
using Hrim.Event.Analytics.Analysis.Models;

namespace Hrim.Event.Analytics.Analysis.Cqrs.GapAnalysis;

public interface IGapCalculationService
{
    /// <summary> Calculates gaps between events </summary>
    /// <param name="events">List of events which should be examined for gaps</param>
    /// <param name="settings">analysis settings (minimal gap)</param>
    /// <returns>Returns Null if no events in the <paramref name="events"/></returns>
    GapAnalysisResult Calculate(List<AnalysisEvent> events, GapSettings settings);
}

public class GapCalculationService: IGapCalculationService
{
    /// <inheritdoc />
    public GapAnalysisResult Calculate(List<AnalysisEvent> events, GapSettings settings) {
        if (events.Count == 0)
            return new GapAnalysisResult(Min: null, MinGapDate: null,
                                         Max: null, MaxGapDate: null,
                                         Avg: null,
                                         GapCount: 0, EventCount: 0);
        var       gapCount   = 0;
        var       gapSum     = TimeSpan.Zero;
        TimeSpan? minGap     = null;
        DateOnly? minGapDate = null;
        TimeSpan? maxGap     = null;
        DateOnly? maxGapDate = null;

        for (var i = 1; i < events.Count; i++) {
            var currentEvent  = events[i];
            var previousEvent = events[i - 1];
            var currentTime   = currentEvent.StartDate.CombineWithTime(currentEvent.StartTime);
            var previousTime  = previousEvent.FinishDate.HasValue
                                    ? previousEvent.FinishDate.CombineWithTime(previousEvent.FinishTime)!.Value
                                    : previousEvent.StartDate.CombineWithTime(previousEvent.StartTime);
            var prevDate  = previousEvent.FinishDate ?? previousEvent.StartDate;
            var gapLength = currentTime - previousTime;
            if (gapLength <= settings.MinimalGap)
                continue;
            gapCount++;
            gapSum += gapLength;
            if (minGap == null || gapLength < minGap) {
                minGap     = gapLength;
                minGapDate = prevDate;
            }
            if (maxGap == null || gapLength > maxGap) {
                maxGap     = gapLength;
                maxGapDate = prevDate;
            }
        }
        TimeSpan? avg = gapCount > 0
                            ? TimeSpan.FromSeconds(Math.Ceiling(gapSum.TotalSeconds / gapCount))
                            : null;
        if(minGap is not null)
            minGap = TimeSpan.FromSeconds(Math.Ceiling(minGap.Value.TotalSeconds));
        if(maxGap is not null)
            maxGap = TimeSpan.FromSeconds(Math.Ceiling(maxGap.Value.TotalSeconds));
        return new GapAnalysisResult(Min: minGap,
                                     MinGapDate: minGapDate,
                                     Max: maxGap,
                                     MaxGapDate: maxGapDate,
                                     Avg: avg,
                                     GapCount: gapCount,
                                     EventCount: events.Count);
    }
}