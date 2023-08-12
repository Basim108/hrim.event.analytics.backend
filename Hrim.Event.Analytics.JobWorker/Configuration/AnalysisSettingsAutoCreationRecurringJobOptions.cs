using Hangfire;

namespace Hrim.Event.Analytics.JobWorker.Configuration;

public record AnalysisSettingsAutoCreationRecurringJobOptions()
    : HrimRecurringJobOptions(CronExpression: Cron.Hourly(),
                              DisplayName: "Analysis Settings Auto Creation Recurring Job",
                              JobId: "analysis-settings-auto-creation-recurring-job",
                              Queue: Queues.ANALYSIS);