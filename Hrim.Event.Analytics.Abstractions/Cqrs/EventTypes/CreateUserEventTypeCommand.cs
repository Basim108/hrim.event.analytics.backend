using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes; 

/// <summary> Creates an instance of a system event type </summary>
/// <param name="EventType">an instance that has to be created</param>
/// <param name="SaveChanges">If true, then changes will be flushed to the storages</param>
/// <param name="Context"><see cref="OperationRequest"/></param>
/// <returns>
/// Null if there is an instance with such name and for the same user in the storage. Code=(409)Conflict.
/// Otherwise returns a created instance with a new ConcurrentToken value. Code=(201)Created
/// </returns>
public record CreateUserEventTypeCommand(UserEventType EventType, bool SaveChanges, OperationContext Context)
    : OperationRequest(Context), IRequest<CqrsResult<UserEventType?>>;
    