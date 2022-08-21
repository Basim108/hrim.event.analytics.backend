using Hrim.Event.Analytics.Abstractions.ViewModels.EventTypes;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;

/// <summary> Returns a union of event types </summary>
public record GetViewEventTypes(Guid CorrelationId, bool IsPublic = true, bool IncludeDeleted = false, Guid? CreatedById = null)
    : BaseRequest(CorrelationId), IRequest<IList<ViewSystemEventType>>;