namespace Hrim.Event.Analytics.Analysis.Cqrs.GapAnalysis.Models;

/// <summary> Result of gap analysis </summary>
/// <param name="Min">Minimal gap between events of a specific event-type. Null if no gaps</param>
/// <param name="MinGapDate">Date when min gap happened. Null if no gaps</param>
/// <param name="Max">Maximal gap between events of a specific event-type. Null if no gaps</param>
/// <param name="MaxGapDate">Date when max gap happened. Null if no gaps</param>
/// <param name="Avg">Average gap between events of a specific event-type. Null if no gaps</param>
/// <param name="GapCount">The total number of gaps found. Zero for no events or no gaps</param>
/// <param name="EventCount">The total number of events participating in the calculation</param>
public record GapAnalysisResult(TimeSpan? Min, DateOnly? MinGapDate, 
                                TimeSpan? Max, DateOnly? MaxGapDate,
                                TimeSpan? Avg, int       GapCount, 
                                int EventCount);