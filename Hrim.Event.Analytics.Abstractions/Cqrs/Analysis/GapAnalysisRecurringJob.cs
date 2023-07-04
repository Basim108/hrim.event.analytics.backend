using Hrim.Event.Analytics.Abstractions.Jobs;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.Analysis;

/// <summary> Initiate gap-analysis calculation </summary>
public record GapAnalysisRecurringJob(Guid CorrelationId): AnalyticsRecurringJob(CorrelationId);
