using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Events;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.Events;

/// <summary> Get all user's occurrence events for a period </summary>
/// <param name="Start">Includes events that equals to a start border </param>
/// <param name="End">Includes events that equals to an end border </param>
/// <param name="Context">
///     <see cref="OperationRequest" />
/// </param>
public record OccurrenceEventGetForPeriod(DateOnly Start, DateOnly End, OperationContext Context)
    : OperationRequest(Context: Context), IRequest<IList<ViewOccurrenceEvent>>;

/// <summary> Get all user's durations events for a period </summary>
/// <param name="Start">Includes durations that intersects start border </param>
/// <param name="End">Includes durations that intersects end border </param>
/// <param name="Context">
///     <see cref="OperationRequest" />
/// </param>
public record DurationEventGetForPeriod(DateOnly Start, DateOnly End, OperationContext Context)
    : OperationRequest(Context: Context), IRequest<IList<ViewDurationEvent>>;