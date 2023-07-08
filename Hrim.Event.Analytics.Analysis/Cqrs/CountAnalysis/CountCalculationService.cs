using Hrim.Event.Analytics.Abstractions.Extensions;
using Hrim.Event.Analytics.Analysis.Cqrs.CountAnalysis.Models;
using Hrim.Event.Analytics.Analysis.Models;

namespace Hrim.Event.Analytics.Analysis.Cqrs.CountAnalysis;

public interface ICountCalculationService
{
    /// <summary> Calculates count of duration events </summary>
    /// <param name="durationEvents">List of events which should be calculated</param>
    /// <param name="occurrenceCount">OccurrenceCount to put into result record</param>
    /// <returns>Returns Null if no events in the <paramref name="durationEvents"/></returns>
    CountAnalysisResult Calculate(List<AnalysisEvent> durationEvents, int occurrenceCount);
}

public class CountCalculationService: ICountCalculationService
{
    /// <inheritdoc />
    public CountAnalysisResult Calculate(List<AnalysisEvent> durationEvents, int occurrenceCount) {
        if (durationEvents.Count == 0)
            return new CountAnalysisResult(MinDuration: null, MinDurationDate: null,
                                           MaxDuration: null, MaxDurationDate: null,
                                           AvgDuration: null, TotalDuration: null,
                                           DurationsCount: durationEvents.Count,
                                           OccurrencesCount: occurrenceCount);
        var       totalSum     = TimeSpan.Zero;
        TimeSpan? minDuration     = null;
        DateOnly? minDurationDate = null;
        TimeSpan? maxDuration     = null;
        DateOnly? maxDurationDate = null;

        for (var i = 0; i < durationEvents.Count; i++) {
            var currentEvent = durationEvents[i];
            var startedAt    = currentEvent.StartDate.CombineWithTime(currentEvent.StartTime);
            var finishedAt = currentEvent.FinishDate.HasValue
                                 ? currentEvent.FinishDate.CombineWithTime(currentEvent.FinishTime)!.Value
                                 : startedAt;
            var eventLength = finishedAt - startedAt;
            totalSum += eventLength;
            if (minDuration == null || eventLength < minDuration) {
                minDuration     = eventLength;
                minDurationDate = currentEvent.StartDate;
            }
            if (maxDuration == null || eventLength > maxDuration) {
                maxDuration     = eventLength;
                maxDurationDate = currentEvent.StartDate;
            }
        }
        TimeSpan? avg = TimeSpan.FromSeconds(Math.Ceiling(totalSum.TotalSeconds / durationEvents.Count()));
        if (minDuration is not null)
            minDuration = TimeSpan.FromSeconds(Math.Ceiling(minDuration.Value.TotalSeconds));
        if (maxDuration is not null)
            maxDuration = TimeSpan.FromSeconds(Math.Ceiling(maxDuration.Value.TotalSeconds));
        return new CountAnalysisResult(MinDuration: minDuration,
                                       MinDurationDate: minDurationDate,
                                       MaxDuration: maxDuration,
                                       MaxDurationDate: maxDurationDate,
                                       AvgDuration: avg,
                                       TotalDuration: totalSum,
                                       DurationsCount: durationEvents.Count,
                                       OccurrencesCount: occurrenceCount);
    }
}