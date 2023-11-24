using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.EfCore;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure;

public class EventTypeAnalysisResultsData
{
    private readonly EventAnalyticDbContext _context;

    public EventTypeAnalysisResultsData(EventAnalyticDbContext context) { _context = context; }

    public StatisticsForEventType EnsureGapExistence(long eventTypeId, string? resultJson = null) {
        resultJson ??= "{\"Min\":\"1.00:00:03\",\"MinGapDate\":\"2023-07-06\",\"Max\":\"1.00:00:04\",\"MaxGapDate\":\"2023-07-05\",\"Avg\":\"1.00:00:03\",\"GapCount\":2,\"EventCount\":3}";
        return EnsureExistence(eventTypeId, FeatureCodes.GAP_ANALYSIS, resultJson);
    }
    
    public StatisticsForEventType EnsureCountExistence(long eventTypeId, string? resultJson = null) {
        resultJson ??= "{\"MinDuration\":\"00:00:00\",\"MinDurationDate\":\"2023-07-06\",\"MaxDuration\":\"00:00:00\",\"MaxDurationDate\":\"2023-07-06\",\"AvgDuration\":\"00:00:00\",\"TotalDuration\":\"00:00:00\",\"OccurrencesCount\":2,\"DurationsCount\":1}";
        return EnsureExistence(eventTypeId, FeatureCodes.COUNT_ANALYSIS, resultJson);
    }
    
    public StatisticsForEventType EnsureExistence(long   eventTypeId,
                                                  string analysisCode,
                                                  string resultJson) {
        var result = new StatisticsForEventType() {
            EntityId     = eventTypeId,
            AnalysisCode = analysisCode,
            ResultJson   = resultJson,
            StartedAt    = DateTime.UtcNow,
            FinishedAt   = DateTime.UtcNow.AddMinutes(1)
        };
        _context.StatisticsForEventTypes.Add(result);
        _context.SaveChanges();
        return result;
    }
}