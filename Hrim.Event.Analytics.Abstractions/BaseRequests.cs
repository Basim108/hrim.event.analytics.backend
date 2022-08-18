namespace Hrim.Event.Analytics.Abstractions;

/// <summary> Requests (commands and queries) </summary>
/// <param name="CorrelationId">Id that will be passed through the whole sequence of commands, queries, jobs, etc</param>
public record BaseRequest(Guid CorrelationId);

/// <summary>
/// Requests (commands and queries) that are made on behave of a specific user
/// </summary>
/// <param name="OperatorId">User id</param>
/// <param name="CorrelationId"></param>
public record UserBaseRequest(Guid OperatorId, Guid CorrelationId)
    : BaseRequest(CorrelationId);