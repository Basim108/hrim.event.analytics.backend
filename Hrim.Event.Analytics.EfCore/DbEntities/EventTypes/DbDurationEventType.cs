using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;

namespace Hrim.Event.Analytics.EfCore.DbEntities.EventTypes;

/// <summary>
/// When it is important to register an event that has start time and end time this system_event_type can be used.
/// <br/>This kind of events may occur several times a day.
/// </summary>
[Obsolete("It's wrong design - must be deleted.")]
public class DbDurationEventType: UserEventType {
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
}