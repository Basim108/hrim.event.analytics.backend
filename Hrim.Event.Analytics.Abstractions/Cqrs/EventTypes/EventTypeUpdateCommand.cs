using Hrim.Event.Analytics.Abstractions.Cqrs.Entity;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;

/// <summary> Updates an instance of the system event type </summary>
/// <param name="EventType">an instance with modified data that has to be stored</param>
/// <param name="SaveChanges">If true, then changes will be flushed to the storages</param>
/// <param name="Context">
///     <see cref="OperationRequest" />
/// </param>
/// <returns>
///     Null when entity is not found (404) or IsDeleted flag of an existed in the storage instance set to true (422).
///     Otherwise returns an updated instance with a new ConcurrentToken value (200).
/// </returns>
/// <remarks>In case you need to set IsDeleted flag to false, use <see cref="RestoreLongEntityCommand{TEntity}" /> CQRS command</remarks>
public record EventTypeUpdateCommand(EventType EventType, bool SaveChanges, OperationContext Context)
    : OperationRequest(Context: Context), IRequest<CqrsResult<EventType?>>;