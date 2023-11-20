using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;

namespace Hrim.Event.Analytics.EfCore.DbEntities.Events;

public class DbBaseEvent: HrimEntity<long>, IHasOwner
{
    /// <summary>
    ///     A user who created an instance of this event type
    /// </summary>
    public HrimUser? CreatedBy { get; set; }

    /// <summary>
    ///     Event type id on which current event is based.
    /// </summary>
    public long EventTypeId { get; set; }

    /// <summary>
    ///     Event type on which current event is based.
    /// </summary>
    public DbEventType? EventType { get; set; }

    /// <summary>
    ///     A user id who created an instance of the event type
    /// </summary>
    public long CreatedById { get; set; }
    
    /// <summary> Analysis results </summary>
    public List<StatisticsForEvent>? AnalysisResults { get; set; }

    /// <summary> Some additional values associated with this event </summary>
    public IDictionary<string, string>? Props { get; set; }
    
    /// <summary> copy all entity properties to the another entity </summary>
    public void CopyTo(BaseEvent another) {
        base.CopyTo(another: another);
        another.EventTypeId = EventTypeId;
        if (EventType != null) {
            another.EventType = new EventType();
            EventType.CopyTo(another.EventType);
        }
        another.CreatedById = CreatedById;
        another.CreatedBy   = CreatedBy;
    }
}