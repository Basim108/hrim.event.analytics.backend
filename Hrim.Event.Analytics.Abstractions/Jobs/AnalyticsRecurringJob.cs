namespace Hrim.Event.Analytics.Abstractions.Jobs;

/// <inheritdoc />
public record AnalyticsRecurringJob(Guid CorrelationId): IAnalyticsJob;