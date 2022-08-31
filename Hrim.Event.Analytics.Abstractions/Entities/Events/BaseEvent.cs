using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;

namespace Hrim.Event.Analytics.Abstractions.Entities.Events;

/// <summary>
/// Properties shared with all types of events
/// </summary>
public abstract class BaseEvent: HrimEntity {
    /// <summary>
    /// A user id who created an instance of the event type
    /// </summary>
    public Guid CreatedById { get; set; }

    /// <summary>
    /// A user who created an instance of this event type
    /// </summary>
    public HrimUser? CreatedBy { get; set; }

    /// <summary>
    /// Event type id on which current event is based.
    /// </summary>
    public Guid EventTypeId { get; set; }

    /// <summary>
    /// Event type on which current event is based.
    /// </summary>
    public UserEventType? EventType { get; set; } = null!;
}