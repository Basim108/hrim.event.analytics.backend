using Hrim.Event.Analytics.Abstractions.Entities.Events;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.Events; 

/// <summary> Creates an instance of occurrence event </summary>
/// <param name="EventInfo">an instance that has to be created</param>
/// <param name="SaveChanges">If true, then changes will be flushed to the storages</param>
/// <param name="CorrelationId"><see cref="BaseRequest"/></param>
/// <returns>
/// Null if there is an instance with such name and for the same user in the storage. Code=(409)Conflict.
/// Otherwise returns a created instance with a new ConcurrentToken value. Code=(201)Created
/// </returns>
public record OccurrenceEventCreateCommand(OccurrenceEvent EventInfo, bool SaveChanges, Guid CorrelationId)
    : BaseRequest(CorrelationId), IRequest<CqrsResult<OccurrenceEvent?>>;
    