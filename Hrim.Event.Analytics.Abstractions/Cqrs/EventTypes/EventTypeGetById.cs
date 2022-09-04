using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;

/// <summary>
/// Returns a full information about a specific event type by its identifier
/// </summary>
/// <param name="Id">Id of a requested event type</param>
/// <param name="IsNotTrackable">If true then does not track changes of the entity properties</param>
/// <param name="Context"><see cref="OperationRequest"/></param>
/// <returns>Null when entity is not found and an entity instance otherwise</returns>
/// <remarks>Have in mind that this request will return an instance of entity even though IsDeleted flag is set to true</remarks>
public record EventTypeGetById(Guid Id, bool IsNotTrackable, OperationContext Context)
    : OperationRequest(Context), IRequest<CqrsResult<UserEventType?>>;