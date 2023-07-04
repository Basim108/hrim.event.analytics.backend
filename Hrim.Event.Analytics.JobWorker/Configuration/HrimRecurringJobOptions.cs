namespace Hrim.Event.Analytics.JobWorker.Configuration;

public record HrimRecurringJobOptions(string CronExpression,
                                  string DisplayName,
                                  string JobId,
                                  string Queue);