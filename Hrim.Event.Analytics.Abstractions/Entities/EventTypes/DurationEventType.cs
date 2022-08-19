namespace Hrim.Event.Analytics.Abstractions.Entities.EventTypes;

/// <summary>
/// When it is important to register an event that has start time and end time this system_event_type can be used.
/// <br/>This kind of events may occur several times a day.
/// </summary>
public class DurationEventType: SystemEventType {
    /// <summary>
    /// Date and time with end-user timezone when an event starts
    /// </summary>
    public DateTimeOffset StartedAt  { get; set; }
    /// <summary>
    /// Date and time with end-user timezone when an event finishes
    /// </summary>
    public DateTimeOffset? FinishedAt { get; set; }
}