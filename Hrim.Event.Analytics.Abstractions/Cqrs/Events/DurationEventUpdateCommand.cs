using Hrim.Event.Analytics.Abstractions.Entities.Events;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.Events; 

/// <summary> Updates an instance of duration event </summary>
/// <param name="EventInfo">an instance that has to be updated</param>
/// <param name="SaveChanges">If true, then changes will be flushed to the storages</param>
/// <param name="Context"><see cref="OperationRequest"/></param>
/// <returns>
/// Null if there is an instance with such name and for the same user in the storage. Code=(409)Conflict.
/// Otherwise returns a created instance with a new ConcurrentToken value. Code=(201)Created
/// </returns>
public record DurationEventUpdateCommand(DurationEvent EventInfo, bool SaveChanges, OperationContext Context)
    : OperationRequest(Context), IRequest<CqrsResult<DurationEvent?>>;
    