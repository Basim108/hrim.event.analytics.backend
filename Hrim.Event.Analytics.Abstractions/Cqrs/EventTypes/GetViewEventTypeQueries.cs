using Hrim.Event.Analytics.Abstractions.ViewModels.EventTypes;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;

/// <summary> Returns a union of event types </summary>
public record GetAllViewEventTypes(Guid CorrelationId, bool IsPublic = true, bool IsDeleted = false, Guid? CreatedById = null)
    : BaseRequest(CorrelationId), IRequest<IList<ViewSystemEventType>>;

/// <summary> Returns duration event types </summary>
public record GetViewDurationEventTypes(Guid CorrelationId, bool IsPublic = true, bool IsDeleted = false, Guid? CreatedById = null)
    : BaseRequest(CorrelationId), IRequest<IList<ViewSystemEventType>>;

/// <summary> Returns occurrence event types </summary>
public record GetViewOccurrenceEventTypes(Guid CorrelationId, bool IsPublic = true, bool IsDeleted = false, Guid? CreatedById = null)
    : BaseRequest(CorrelationId), IRequest<IList<ViewSystemEventType>>;