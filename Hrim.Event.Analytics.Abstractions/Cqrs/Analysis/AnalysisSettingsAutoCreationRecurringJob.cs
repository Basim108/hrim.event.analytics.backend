using Hrim.Event.Analytics.Abstractions.Jobs;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.Analysis;

/// <summary> Check all kinds of analysis settings for each event type and if there is no settings - creates it. </summary>
public record AnalysisSettingsAutoCreationRecurringJob(): AnalyticsRecurringJob();
