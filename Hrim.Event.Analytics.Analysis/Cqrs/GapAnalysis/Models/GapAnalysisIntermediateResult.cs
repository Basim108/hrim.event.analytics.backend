namespace Hrim.Event.Analytics.Analysis.Cqrs.GapAnalysis.Models;

/// <summary>
/// Intermediate gap analysis result.
/// Mostly used for calculating separately analysis for different kind of events (duration/occurrence)
/// </summary>
/// <param name="Min">Minimal gap between events of a specific event-type. Null if no gaps</param>
/// <param name="MinGapDate">Date when min gap happened. Null if no gaps</param>
/// <param name="Max">Maximal gap between events of a specific event-type. Null if no gaps</param>
/// <param name="MaxGapDate">Date when max gap happened. Null if no gaps</param>
/// <param name="Count">
/// Number of events that participating in gap sum.
/// For example, there are 3 events in the list and no gaps between them, then this property will be 0.
/// If there is only one gap between 2nd and 3d event, then this property equals to 1. 
/// </param>
/// <param name="Sum">Sum of gaps between all participating events</param>
public record GapAnalysisIntermediateResult(TimeSpan? Min, DateOnly? MinGapDate, 
                                            TimeSpan? Max, DateOnly? MaxGapDate,
                                            int Count, TimeSpan Sum);