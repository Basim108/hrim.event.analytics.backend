namespace Hrim.Event.Analytics.Abstractions.ViewModels.EventTypes;

/// <summary>
/// Model to present base properties of any derived event type.
/// <br/>https://hrimsoft.atlassian.net/wiki/spaces/HRIMCALEND/pages/65566/System+Event+Types
/// </summary>
/// <param name="Name">User event type name, e.g. 'nice mood', 'headache', etc</param>
/// <param name="Description">Description given by user, when user_event_type based on this one will be created.</param>
/// <param name="Color">A color that events will be drawing with in a calendar.</param>
/// <param name="IsPublic">An owner who created this user_event_type could share it with other end-users</param>
/// <param name="IsDeleted">Soft deletion flag</param>
/// <param name="Id">Entity id</param>
public record ViewEventType(Guid    Id,
                            string  Name,
                            string? Description,
                            string  Color,
                            bool    IsPublic,
                            bool    IsDeleted);