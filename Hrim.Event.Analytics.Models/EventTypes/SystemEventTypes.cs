using Hrim.Event.Analytics.Models.Entities;

namespace Hrim.Event.Analytics.Models.EventTypes;

/// <summary>
/// Base events is an abstraction that shares features and behaviour among all other event_types.
/// <br/>https://hrimsoft.atlassian.net/wiki/spaces/HRIMCALEND/pages/65566/System+Event+Types
/// </summary>
/// <param name="Name">User event type name, e.g. 'nice mood', 'headache', etc</param>
/// <param name="Description">Description given by user, when user_event_type based on this one will be created.</param>
/// <param name="Color">A color that events will be drawing with in a calendar.</param>
/// <param name="Tags">A list of tags that will be automatically set to an each event</param>
/// <param name="IsPublic">An owner who created this user_event_type could share it with other end-users</param>
/// <param name="Id">Entity id</param>
/// <param name="CreatedAt">Date and UTC time of event_type creation</param>
/// <param name="UpdateAt">Date and UTC time of event_type last update</param>
/// <param name="IsDeleted">Soft deletion flag</param>
/// <param name="ConcurrentToken">Update is possible only this token equals to the token in the storage</param>
public abstract record SystemEventType(Guid          Id,
                                       string        Name,
                                       string        Description,
                                       string        Color,
                                       IList<string> Tags,
                                       DateTime      CreatedAt,
                                       DateTime?     UpdateAt,
                                       bool          IsDeleted,
                                       bool          IsPublic,
                                       int           ConcurrentToken)
    : Entity(Id, CreatedAt, UpdateAt, IsDeleted, ConcurrentToken);

/// <summary>
/// When the main importance is the fact that an event occurred.
/// <br/>This kind of events may occur several times a day.
/// </summary>
/// <param name="OccurredAt">Date and time with end-user timezone when an event occurred</param>
/// <param name="Name">User event type name, e.g. 'nice mood', 'headache', etc</param>
/// <param name="Description">Description given by user, when user_event_type based on this one will be created.</param>
/// <param name="Color">A color that events will be drawing with in a calendar.</param>
/// <param name="Tags">A list of tags that will be automatically set to an each event</param>
/// <param name="IsPublic">An owner who created this user_event_type could share it with other end-users</param>
/// <param name="Id">Entity id</param>
/// <param name="CreatedAt">Date and UTC time of event_type creation</param>
/// <param name="UpdateAt">Date and UTC time of event_type last update</param>
/// <param name="IsDeleted">Soft deletion flag</param>
/// <param name="ConcurrentToken">Update is possible only this token equals to the token in the storage</param>
public record OccurrenceEventType(Guid           Id,
                                  DateTimeOffset OccurredAt,
                                  string         Name,
                                  string         Description,
                                  string         Color,
                                  IList<string>  Tags,
                                  DateTime       CreatedAt,
                                  DateTime?      UpdateAt,
                                  bool           IsDeleted,
                                  bool           IsPublic,
                                  int            ConcurrentToken)
    : SystemEventType(Id, Name, Description, Color, Tags, CreatedAt, UpdateAt, IsDeleted, IsPublic, ConcurrentToken);

/// <summary>
/// When it is important to register an event that has start time and end time this system_event_type can be used.
/// <br/>This kind of events may occur several times a day.
/// </summary>
/// <param name="StartedAt">Date and time with end-user timezone when en event starts</param>
/// <param name="FinishedAt">Date and time with end-user timezone when an event finishes</param>
/// <param name="Name">User event type name, e.g. 'nice mood', 'headache', etc</param>
/// <param name="Description">Description given by user, when user_event_type based on this one will be created.</param>
/// <param name="Color">A color that events will be drawing with in a calendar.</param>
/// <param name="Tags">A list of tags that will be automatically set to an each event</param>
/// <param name="IsPublic">An owner who created this user_event_type could share it with other end-users</param>
/// <param name="Id">Entity id</param>
/// <param name="CreatedAt">Date and UTC time of event_type creation</param>
/// <param name="UpdateAt">Date and UTC time of event_type last update</param>
/// <param name="IsDeleted">Soft deletion flag</param>
/// <param name="ConcurrentToken">Update is possible only this token equals to the token in the storage</param>
public record DurationEventType(Guid           Id,
                                DateTimeOffset StartedAt,
                                DateTimeOffset FinishedAt,
                                string         Name,
                                string         Description,
                                string         Color,
                                IList<string>  Tags,
                                DateTime       CreatedAt,
                                DateTime?      UpdateAt,
                                bool           IsDeleted,
                                bool           IsPublic,
                                int            ConcurrentToken)
    : SystemEventType(Id, Name, Description, Color, Tags, CreatedAt, UpdateAt, IsDeleted, IsPublic, ConcurrentToken);