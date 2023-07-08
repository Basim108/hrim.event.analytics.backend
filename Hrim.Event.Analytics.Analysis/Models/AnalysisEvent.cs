namespace Hrim.Event.Analytics.Analysis.Models;

/// <summary> Model of input duration/occurrence events </summary>
/// <param name="StartDate"></param>
/// <param name="StartTime"></param>
/// <param name="FinishDate">For occurrence that will be equal to StartDate</param>
/// <param name="FinishTime">For occurrence that will be equal to StartTime</param>
public record AnalysisEvent(DateOnly StartDate, DateTimeOffset StartTime, DateOnly? FinishDate, DateTimeOffset? FinishTime);