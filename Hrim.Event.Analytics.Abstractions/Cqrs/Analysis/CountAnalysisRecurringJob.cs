using Hrim.Event.Analytics.Abstractions.Jobs;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.Analysis;

/// <summary> Initiate count-analysis calculation </summary>
public record CountAnalysisRecurringJob(): AnalyticsRecurringJob();
