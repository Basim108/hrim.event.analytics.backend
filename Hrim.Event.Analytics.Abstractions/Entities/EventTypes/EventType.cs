using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
#pragma warning disable CS8618

namespace Hrim.Event.Analytics.Abstractions.Entities.EventTypes;

/// <summary>
///     Event types created by user
///     <br />https://hrimsoft.atlassian.net/wiki/spaces/HRIMCALEND/pages/65566/System+Event+Types
/// </summary>
public class EventType: HrimEntity<long>, IHasOwner
{
    /// <summary>
    ///     Event type name, e.g. 'nice mood', 'headache', etc
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Description given by user, when user_event_type based on this one will be created.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     A color that events will be drawing with in a calendar. e.g. 'red', '#ff0000'
    /// </summary>
    public string Color { get; set; }

    /// <summary>
    ///     A user who created an instance of this event type
    /// </summary>
    public virtual HrimUser? CreatedBy { get; set; }

    /// <summary>
    ///     An owner who created this event_type could share it with other end-users
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    ///     A user id who created an instance of the event type
    /// </summary>
    public long CreatedById { get; set; }
    
    /// <summary> Analysis settings related to this event type </summary>
    public List<AnalysisConfigByEventType>? AnalysisSettings { get; set; }
    
    /// <summary> Analysis results </summary>
    public List<StatisticsForEventType>? AnalysisResults { get; set; }

    /// <summary>
    /// Reference to a more general event type, which this type is specified in some context
    /// For example, if current event type is Hatha Yoga, its parent type might be just general Yoga. 
    /// </summary>
    public EventType? Parent { get; set; }

    /// <summary> Identifier of parent event type </summary>
    public long? ParentId { get; set; }
    
    /// <summary> copy all entity properties to the another entity </summary>
    public void CopyTo(EventType another) {
        base.CopyTo(another: another);
        another.Name        = Name;
        another.Description = Description;
        another.Color       = Color;
        another.CreatedById = CreatedById;
        another.CreatedBy   = CreatedBy;
        another.IsPublic    = IsPublic;
    }
}