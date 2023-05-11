using Hrim.Event.Analytics.Abstractions.Entities.Events;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.Events;

/// <summary>
///     Returns a full information about a specific event by its identifier
/// </summary>
/// <param name="Id">Id of a requested event</param>
/// <param name="IsNotTrackable">If true then does not track changes of the entity properties</param>
/// <param name="Context">
///     <see cref="OperationRequest" />
/// </param>
/// <returns>Null when entity is not found and an entity instance otherwise</returns>
/// <remarks>Have in mind that this request will return an instance of entity even though IsDeleted flag is set to true</remarks>
public record GetEventById<TEvent>(Guid Id, bool IsNotTrackable, OperationContext Context)
    : OperationRequest(Context: Context), IRequest<CqrsResult<TEvent?>>
    where TEvent : BaseEvent, new();