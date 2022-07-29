using Hrim.Event.Analytics.Models.EventTypes;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.EventTypes; 

public record CreateUserEventType(SystemEventType EventType, Guid CorrelationId)
    : BaseRequest(CorrelationId), IRequest<SystemEventType>;