namespace Hrim.Event.Analytics.Api.V1.Models;

/// <summary> Get all events for a period </summary>
/// <param name="Start">Includes durations that intersects start border </param>
/// <param name="End">Includes durations that intersects end border </param>
/// <param name="OwnerId">Returns only those events that were created by this user id</param>
public record EventRequest(DateOnly Start, DateOnly End, Guid OwnerId);
