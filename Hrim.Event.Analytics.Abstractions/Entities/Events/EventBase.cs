namespace Hrim.Event.Analytics.Abstractions.Entities.Events; 

/// <summary>
/// Properties shared with all types of events
/// </summary>
public abstract class EventBase: Entity {
    /// <summary>
    /// A user id who created an instance of the event type
    /// </summary>
    public Guid CreatedById { get; set; }

    /// <summary>
    /// A user who created an instance of this event type
    /// </summary>
    public HrimUser? CreatedBy { get; set; }

    /// <summary>
    /// An owner who created this event_type could share it with other end-users.
    /// Will override IsPublic value of an event_type instance
    /// </summary>
    public bool IsPublic { get; set; }
}