namespace Hrim.Event.Analytics.Abstractions.Cqrs;

/// <summary></summary>
/// <param name="UserId"></param>
/// <param name="CorrelationId">Id that will be passed through the whole sequence of commands, queries, jobs, etc</param>
public record OperationContext(Guid UserId, Guid CorrelationId);