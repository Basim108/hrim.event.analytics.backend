namespace Hrim.Event.Analytics.Abstractions.ViewModels.EventTypes;

/// <summary>
/// When it is important to register an event that has start time and end time this system_event_type can be used.
/// <br/>This kind of events may occur several times a day.
/// </summary>
/// <param name="StartedAt">Date and time with end-user timezone when en event starts</param>
/// <param name="FinishedAt">Date and time with end-user timezone when an event finishes</param>
/// <param name="Name">User event type name, e.g. 'nice mood', 'headache', etc</param>
/// <param name="Description">Description given by user, when user_event_type based on this one will be created.</param>
/// <param name="Color">A color that events will be drawing with in a calendar.</param>
/// <param name="IsPublic">An owner who created this user_event_type could share it with other end-users</param>
/// <param name="Id">Entity id</param>
public record ViewDurationEventType(Guid           Id,
                                    DateTimeOffset StartedAt,
                                    DateTimeOffset FinishedAt,
                                    string         Name,
                                    string?         Description,
                                    string         Color,
                                    bool           IsPublic)
    : ViewSystemEventType(Id, Name, Description, Color, IsPublic);