namespace Hrim.Event.Analytics.Abstractions.ViewModels.EventTypes;

/// <summary>
/// When the main importance is the fact that an event occurred.
/// <br/>This kind of events may occur several times a day.
/// </summary>
/// <param name="OccurredAt">Date and time with end-user timezone when an event occurred</param>
/// <param name="Name">User event type name, e.g. 'nice mood', 'headache', etc</param>
/// <param name="Description">Description given by user, when user_event_type based on this one will be created.</param>
/// <param name="Color">A color that events will be drawing with in a calendar.</param>
/// <param name="IsPublic">An owner who created this user_event_type could share it with other end-users</param>
/// <param name="Id">Entity id</param>
public record ViewOccurrenceEventType(Guid           Id,
                                      DateTimeOffset OccurredAt,
                                      string         Name,
                                      string?        Description,
                                      string         Color,
                                      bool           IsPublic);