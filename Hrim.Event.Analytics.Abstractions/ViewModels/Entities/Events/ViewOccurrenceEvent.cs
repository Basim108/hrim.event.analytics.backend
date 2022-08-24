namespace Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Events;

/// <summary>
/// When the main importance is the fact that an event occurred.
/// <br/>This kind of events may occur several times a day.
/// </summary>
/// <param name="OccurredAt">Date and time with end-user timezone when an event occurred</param>
/// <param name="Id">Event id</param>
/// <param name="EventTypeId">Id of event type on which this event is based on</param>
public record ViewOccurrenceEvent(Guid Id, DateTimeOffset OccurredAt, Guid EventTypeId);