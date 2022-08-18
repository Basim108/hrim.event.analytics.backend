using Hrim.Event.Analytics.Abstractions.ViewModels.EventTypes;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;

/// <summary> Returns a union of all public and not deleted event types </summary>
public record GetAllPublicViewEventTypes(Guid CorrelationId)
    :BaseRequest(CorrelationId), IRequest<IEnumerable<ViewSystemEventType>>;