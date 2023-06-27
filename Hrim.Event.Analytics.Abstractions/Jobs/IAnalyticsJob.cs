using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Jobs;

/// <summary>
/// Each analytics job should implement this interface
/// </summary>
public interface IAnalyticsJob: IRequest
{
    /// <summary>
    /// Correlation Id that can be serialized from enqueueing process, and
    /// then deserialized by hangfire and creates a correlation scope with it.
    /// </summary>
    string CorrelationId { get; }
}