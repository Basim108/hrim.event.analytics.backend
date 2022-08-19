using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;

namespace Hrim.Event.Analytics.EfCore.DbEntities.EventTypes;

/// <summary>
/// When the main importance is the fact that an event occurred.
/// <br/>This kind of events may occur several times a day.
/// </summary>
public class DbOccurrenceEventType: SystemEventType {
    /// <summary>
    /// Date and time with end-user timezone when an event occurred
    /// </summary>
    public DateOnly OccurredOn { get; set; }

    /// <summary>
    /// Time with end-user timezone when an event occurred
    /// </summary>
    public DateTimeOffset OccurredAt { get; set; }
}