using Hrim.Event.Analytics.Abstractions.Entities.Events;

namespace Hrim.Event.Analytics.EfCore.DbEntities.Events;

/// <summary>
/// When it is important to register an event that has start time and end time this system_event_type can be used.
/// <br/>This kind of events may occur several times a day and can cross each other.
/// </summary>
public class DbDurationEvent: BaseEvent {
    /// <summary>
    /// Date when an event started
    /// </summary>
    public DateOnly StartedOn { get; set; }

    /// <summary>
    /// Time with end-user timezone when an event started
    /// </summary>
    public DateTimeOffset StartedAt { get; set; }

    /// <summary>
    /// Date when an event finished
    /// </summary>
    public DateOnly? FinishedOn { get; set; }

    /// <summary>
    /// Time with end-user timezone when an event finished
    /// </summary>
    public DateTimeOffset? FinishedAt { get; set; }

    /// <summary> copy all entity properties to the another entity </summary>
    public void CopyTo(DurationEvent another) {
        base.CopyTo(another);
        another.StartedAt = new DateTimeOffset(StartedOn.Year, StartedOn.Month, StartedOn.Day,
                                               StartedAt.Hour, StartedAt.Minute, StartedAt.Second, StartedAt.Millisecond,
                                               StartedAt.Offset);

        another.FinishedAt = FinishedOn.HasValue
                                 ? new DateTimeOffset(FinishedOn.Value.Year, FinishedOn.Value.Month, FinishedOn.Value.Day,
                                                      FinishedAt!.Value.Hour, FinishedAt.Value.Minute, FinishedAt.Value.Second,
                                                      FinishedAt.Value.Millisecond, FinishedAt.Value.Offset)
                                 : null;
    }
}