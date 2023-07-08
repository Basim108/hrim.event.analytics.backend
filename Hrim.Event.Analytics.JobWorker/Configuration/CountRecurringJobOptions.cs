namespace Hrim.Event.Analytics.JobWorker.Configuration;

public record CountRecurringJobOptions()
    : HrimRecurringJobOptions(CronExpression: "*/45 * * * *",
                              DisplayName: "Count Analysis Recurring Job",
                              JobId: "count-analysis-recurring-job",
                              Queue: Queues.ANALYSIS);