namespace Hrim.Event.Analytics.Abstractions.Entities.Events;

/// <summary>
///     When it is important to register an event that has start time and end time this system_event_type can be used.
///     <br />This kind of events may occur several times a day and can cross each other.
/// </summary>
public class DurationEvent: BaseEvent
{
    /// <summary>
    ///     Date and time with end-user timezone when an event starts
    /// </summary>
    public DateTimeOffset StartedAt { get; set; }

    /// <summary>
    ///     Date and time with end-user timezone when an event finishes
    /// </summary>
    public DateTimeOffset? FinishedAt { get; set; }

    /// <summary> copy all entity properties to the another entity </summary>
    public void CopyTo(DurationEvent another) {
        base.CopyTo(another: another);
        another.StartedAt  = StartedAt;
        another.FinishedAt = FinishedAt;
    }
}