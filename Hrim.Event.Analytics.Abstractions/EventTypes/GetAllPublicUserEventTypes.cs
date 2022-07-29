using Hrim.Event.Analytics.Models.EventTypes;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.EventTypes;

public record GetAllPublicUserEventTypes(Guid CorrelationId)
    :BaseRequest(CorrelationId), IRequest<IEnumerable<SystemEventType>>;