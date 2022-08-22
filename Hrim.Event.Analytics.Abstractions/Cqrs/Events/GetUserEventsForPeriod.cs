using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.ViewModels.Events;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.Events;

/// <summary> Get all user's occurrence events for a period </summary>
/// <param name="Start">Includes events that equals to a start border </param>
/// <param name="End">Includes events that equals to an end border </param>
/// <param name="OwnerId">Returns only those events that were created by this user id</param>
/// <param name="CorrelationId"><see cref="BaseRequest"/></param>
public record GetUserOccurrencesForPeriod(DateOnly Start, DateOnly End, Guid OwnerId, Guid CorrelationId)
    : BaseRequest(CorrelationId), IRequest<IList<ViewOccurrenceEvent>>;
    
/// <summary> Get all user's durations events for a period </summary>
/// <param name="Start">Includes durations that intersects start border </param>
/// <param name="End">Includes durations that intersects end border </param>
/// <param name="OwnerId">Returns only those events that were created by this user id</param>
/// <param name="CorrelationId"><see cref="BaseRequest"/></param>
public record GetUserDurationsForPeriod(DateOnly Start, DateOnly End, Guid OwnerId, Guid CorrelationId)
    : BaseRequest(CorrelationId), IRequest<IList<ViewDurationEvent>>;