using Hrim.Event.Analytics.Abstractions.Entities.Account;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.Users; 

/// <summary> Creates a hrim user instance </summary>
public record HrimUserCreateCommand(Guid CorrelationId, bool SaveChanges)
    :BaseRequest(CorrelationId), IRequest<HrimUser>;