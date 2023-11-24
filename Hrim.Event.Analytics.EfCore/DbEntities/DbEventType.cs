using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.EfCore.DbEntities.Analysis;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.EfCore.DbEntities;

public class DbEventType: HrimEntity<long>, IHasOwner
{
    public LTree? TreeNodePath { get; set; }

    /// <summary>
    /// Reference to a more general event type, which this type is specified in some context
    /// For example, if current event type is Hatha Yoga, its parent type might be just general Yoga. 
    /// </summary>
    public DbEventType? Parent { get; set; }

    /// <summary> Identifier of parent event type </summary>
    public long? ParentId { get; set; }

    /// <summary>
    /// Reference to a list of mre specific event types
    /// For example, if current event type is smoking, then its children subtypes might be "smoking cigarettes", "vaping"  
    /// </summary>
    public List<DbEventType>? Children { get; set; }

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
    public HrimUser? CreatedBy { get; set; }

    /// <summary>
    ///     An owner who created this event_type could share it with other end-users
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    ///     A user id who created an instance of the event type
    /// </summary>
    public long CreatedById { get; set; }

    /// <summary> Analysis settings related to this event type </summary>
    public List<DbAnalysisConfigByEventType>? AnalysisSettings { get; set; }

    /// <summary> Analysis results </summary>
    public List<StatisticsForEventType>? AnalysisResults { get; set; }

    public void GeneratePath() {
        if (Id <= 0)
            throw new ArgumentException($"Cannot generate ltree path when id is not a positive integer. Id={Id}", nameof(Id));
        
        TreeNodePath = Parent == null
                           ? Id.ToString()
                           : $"{Parent.TreeNodePath}.{Id}";
    }
}