using Hangfire;

namespace Hrim.Event.Analytics.JobWorker.Configuration;

public record GapHrimRecurringJobOptions()
    : HrimRecurringJobOptions(CronExpression: Cron.Hourly(),
                              DisplayName: "Gap Analysis Recurring Job",
                              JobId: "gap-analysis-recurring-job",
                              Queue: Queues.ANALYSIS);