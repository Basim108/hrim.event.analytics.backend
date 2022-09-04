namespace Hrim.Event.Analytics.Abstractions.Entities.Events;

/// <summary>
/// When the main importance is the fact that an event occurred.
/// <br/>This kind of events may occur several times a day.
/// </summary>
public class OccurrenceEvent: BaseEvent {
    /// <summary>
    /// Date and time with end-user timezone when an event occurred
    /// </summary>
    public DateTimeOffset OccurredAt { get; set; }
    
    /// <summary> copy all entity properties to the another entity </summary>
    public void CopyTo(OccurrenceEvent another) {
        base.CopyTo(another);
        another.OccurredAt = OccurredAt;
    }
}