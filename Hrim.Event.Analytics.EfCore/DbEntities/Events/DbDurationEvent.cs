using Hrim.Event.Analytics.Abstractions.Entities.Events;

namespace Hrim.Event.Analytics.EfCore.DbEntities.Events;

/// <summary>
///     When it is important to register an event that has start time and end time this system_event_type can be used.
///     <br />This kind of events may occur several times a day and can cross each other.
/// </summary>
public class DbDurationEvent: DbBaseEvent
{
    /// <summary>
    ///     Date when an event started
    /// </summary>
    public DateOnly StartedOn { get; set; }

    /// <summary>
    ///     Time with end-user timezone when an event started
    /// </summary>
    public DateTimeOffset StartedAt { get; set; }

    /// <summary>
    ///     Date when an event finished
    /// </summary>
    public DateOnly? FinishedOn { get; set; }

    /// <summary>
    ///     Time with end-user timezone when an event finished
    /// </summary>
    public DateTimeOffset? FinishedAt { get; set; }

    /// <summary> copy all entity properties to the another entity </summary>
    /// TODO: Remove this cloning logic to AutoMapper
    public void CopyTo(DurationEvent another) {
        base.CopyTo(another: another);
        another.StartedAt = new DateTimeOffset(year: StartedOn.Year,
                                               month: StartedOn.Month,
                                               day: StartedOn.Day,
                                               hour: StartedAt.Hour,
                                               minute: StartedAt.Minute,
                                               second: StartedAt.Second,
                                               millisecond: StartedAt.Millisecond,
                                               offset: StartedAt.Offset);

        another.FinishedAt = FinishedOn.HasValue
                                 ? new DateTimeOffset(year: FinishedOn.Value.Year,
                                                      month: FinishedOn.Value.Month,
                                                      day: FinishedOn.Value.Day,
                                                      hour: FinishedAt!.Value.Hour,
                                                      minute: FinishedAt.Value.Minute,
                                                      second: FinishedAt.Value.Second,
                                                      millisecond: FinishedAt.Value.Millisecond,
                                                      offset: FinishedAt.Value.Offset)
                                 : null;
    }
}