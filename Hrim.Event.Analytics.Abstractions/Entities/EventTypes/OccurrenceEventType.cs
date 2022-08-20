namespace Hrim.Event.Analytics.Abstractions.Entities.EventTypes;

/// <summary>
/// When the main importance is the fact that an event occurred.
/// <br/>This kind of events may occur several times a day.
/// </summary>
[Obsolete("It's wrong design - must be deleted.")]
public class OccurrenceEventType: SystemEventType {
    /// <summary>
    /// Date and time with end-user timezone when an event occurred
    /// </summary>
    public DateTimeOffset OccurredAt { get; set; }
}