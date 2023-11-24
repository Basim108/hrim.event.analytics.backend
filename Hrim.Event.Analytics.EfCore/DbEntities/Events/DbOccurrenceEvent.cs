using Hrim.Event.Analytics.Abstractions.Entities.Events;

namespace Hrim.Event.Analytics.EfCore.DbEntities.Events;

/// <summary>
///     When the main importance is the fact that an event occurred.
///     <br />This kind of events may occur several times a day.
/// </summary>
public class DbOccurrenceEvent: DbBaseEvent
{
    /// <summary>
    ///     Date and time with end-user timezone when an event occurred
    /// </summary>
    public DateOnly OccurredOn { get; set; }

    /// <summary>
    ///     Time with end-user timezone when an event occurred
    /// </summary>
    public DateTimeOffset OccurredAt { get; set; }

    /// <summary> copy all entity properties to the another entity </summary>
    public void CopyTo(OccurrenceEvent another) {
        base.CopyTo(another: another);
        another.OccurredAt = new DateTimeOffset(year: OccurredOn.Year,
                                                month: OccurredOn.Month,
                                                day: OccurredOn.Day,
                                                hour: OccurredAt.Hour,
                                                minute: OccurredAt.Minute,
                                                second: OccurredAt.Second,
                                                millisecond: OccurredAt.Millisecond,
                                                offset: OccurredAt.Offset);
    }
}