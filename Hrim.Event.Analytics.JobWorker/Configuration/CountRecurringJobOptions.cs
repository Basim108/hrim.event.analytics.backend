using System.Diagnostics.CodeAnalysis;

namespace Hrim.Event.Analytics.JobWorker.Configuration;

[ExcludeFromCodeCoverage]
public record CountRecurringJobOptions()
    : HrimRecurringJobOptions(CronExpression: "*/45 * * * *",
                              DisplayName: "Count Analysis Recurring Job",
                              JobId: "count-analysis-recurring-job",
                              Queue: Queues.ANALYSIS);