namespace Hrim.Event.Analytics.Abstractions.ViewModels.Entities.EventTypes; 

/// <summary>
/// Model for event type creation
/// </summary>
public class UpdateEventTypeRequest {
    /// <summary> Event type id </summary>
    public Guid Id { get; set; }
    
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
    /// An owner who created this event_type could share it with other end-users
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// Update is possible only when this token equals to the token in the storage
    /// </summary>
    public long ConcurrentToken { get; set; }
}