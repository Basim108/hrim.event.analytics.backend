using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Events;

namespace Hrim.Event.Analytics.Api.V1.Models;

/// <summary> Response for a period request </summary>
/// <param name="Request">A request for with this response</param>
/// <param name="Occurrences">occurrence events that satisfy a request</param>
/// <param name="Durations">duration events that satisfy a request</param>
public record EventsForPeriodResponse(GetEventsForPeriodRequest Request, IList<ViewOccurrenceEvent>? Occurrences, IList<ViewDurationEvent>? Durations);