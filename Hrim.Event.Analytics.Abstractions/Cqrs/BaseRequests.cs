namespace Hrim.Event.Analytics.Abstractions.Cqrs;

/// <summary> Requests (commands and queries) </summary>
public record BaseRequest(Guid CorrelationId);

/// <summary>
///     Requests (commands and queries) that initiated by an end-user
/// </summary>
/// <param name="Context"></param>
public record OperationRequest(OperationContext Context);