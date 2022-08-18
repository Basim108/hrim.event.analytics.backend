namespace Hrim.Event.Analytics.Abstractions.Entities.EventTypes;

/// <summary>
/// Base events is an abstraction that shares features and behaviour among all other event_types.
/// <br/>https://hrimsoft.atlassian.net/wiki/spaces/HRIMCALEND/pages/65566/System+Event+Types
/// </summary>
public abstract class SystemEventType: Entity {
    /// <summary>
    /// Event type name, e.g. 'nice mood', 'headache', etc
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Description given by user, when user_event_type based on this one will be created.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// A color that events will be drawing with in a calendar. e.g. 'red', '#ff0000'
    /// </summary>
    public string Color { get; set; } = null!;

    /// <summary>
    /// A user id who created an instance of the event type
    /// </summary>
    public Guid CreatedById { get; set; }

    /// <summary>
    /// A user who created an instance of this event type
    /// </summary>
    public HrimUser CreatedBy { get; set; } = null!;

    /// <summary>
    /// An owner who created this event_type could share it with other end-users
    /// </summary>
    public bool IsPublic { get; set; }
}