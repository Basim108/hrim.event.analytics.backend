using Hrim.Event.Analytics.Models.EventTypes;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.EventTypes;

public record GetUserEventTypeById(Guid EventTypeId, Guid CorrelationId)
    : BaseRequest(CorrelationId), IRequest<SystemEventType>;