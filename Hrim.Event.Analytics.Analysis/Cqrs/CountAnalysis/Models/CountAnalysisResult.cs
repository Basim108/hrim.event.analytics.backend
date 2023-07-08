namespace Hrim.Event.Analytics.Analysis.Cqrs.CountAnalysis.Models;

/// <summary> Result of gap analysis </summary>
/// <param name="MinDuration">Minimal duration. Null if no events</param>
/// <param name="MinDurationDate">Date when min duration happened. Null if no events</param>
/// <param name="MaxDuration">Maximal duration. Null if no events</param>
/// <param name="MaxDurationDate">Date when max duration happened. Null if no events</param>
/// <param name="AvgDuration">Average duration. Null if no events</param>
/// <param name="TotalDuration">Sum of all duration events. Null if no events</param>
/// <param name="OccurrencesCount">The total number of occurrence events. Zero for no events</param>
/// <param name="DurationsCount">The total number of duration events. Zero for no events</param>
public record CountAnalysisResult(TimeSpan? MinDuration,
                                  DateOnly? MinDurationDate,
                                  TimeSpan? MaxDuration,
                                  DateOnly? MaxDurationDate,
                                  TimeSpan? AvgDuration,
                                  TimeSpan? TotalDuration,
                                  int       OccurrencesCount,
                                  int       DurationsCount);