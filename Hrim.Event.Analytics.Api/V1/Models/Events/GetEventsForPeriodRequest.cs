namespace Hrim.Event.Analytics.Api.V1.Models;

/// <summary> Get all events for a period </summary>
/// <param name="Start">Includes durations that intersects start border </param>
/// <param name="End">Includes durations that intersects end border </param>
public record GetEventsForPeriodRequest(DateOnly Start, DateOnly End);