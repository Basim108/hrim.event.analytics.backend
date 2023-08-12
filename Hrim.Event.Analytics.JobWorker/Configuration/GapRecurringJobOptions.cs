using System.Diagnostics.CodeAnalysis;
using Hangfire;

namespace Hrim.Event.Analytics.JobWorker.Configuration;

[ExcludeFromCodeCoverage]
public record GapRecurringJobOptions()
    : HrimRecurringJobOptions(CronExpression: Cron.Hourly(),
                              DisplayName: "Gap Analysis Recurring Job",
                              JobId: "gap-analysis-recurring-job",
                              Queue: Queues.ANALYSIS);