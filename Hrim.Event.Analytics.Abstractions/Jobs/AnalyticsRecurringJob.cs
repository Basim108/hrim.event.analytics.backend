namespace Hrim.Event.Analytics.Abstractions.Jobs;

/// <inheritdoc />
public record AnalyticsRecurringJob(): IAnalyticsJob
{
    /// <inheritdoc />
    public Guid CorrelationId { get; } = Guid.NewGuid();
}