using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.EventTypes;

namespace Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Events;

/// <summary>
///     When the main importance is the fact that an event occurred.
///     <br />This kind of events may occur several times a day.
/// </summary>
/// <param name="Id">Event id</param>
/// <param name="OccurredAt">Date and time with end-user timezone when an event occurred</param>
/// <param name="EventType">Event type on which this event is based on</param>
/// <param name="ConcurrentToken">Update is possible only when this token equals to the token in the storage</param>
/// <param name="Props">Additional properties for a particular event</param>
public record ViewOccurrenceEvent(long           Id,
                                  DateTimeOffset OccurredAt,
                                  ViewEventType  EventType,
                                  long           ConcurrentToken,
                                  IDictionary<string, string>? Props = null);