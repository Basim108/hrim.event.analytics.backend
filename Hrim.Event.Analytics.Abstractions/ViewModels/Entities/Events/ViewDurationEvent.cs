namespace Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Events;

/// <summary>
/// When it is important to register an event that has start time and end time this system_event_type can be used.
/// <br/>This kind of events may occur several times a day.
/// </summary>
/// <param name="StartedAt">Date and time with end-user timezone when en event starts</param>
/// <param name="FinishedAt">Date and time with end-user timezone when an event finishes</param>
/// <param name="Id">Event id</param>
/// <param name="EventTypeId">Id of event type on which this event is based on</param>
/// <param name="Color">color from event type</param>
/// <param name="ConcurrentToken">Update is possible only when this token equals to the token in the storage</param>
public record ViewDurationEvent(Guid            Id,
                                DateTimeOffset  StartedAt,
                                DateTimeOffset? FinishedAt,
                                Guid            EventTypeId,
                                string          Color,
                                long            ConcurrentToken);