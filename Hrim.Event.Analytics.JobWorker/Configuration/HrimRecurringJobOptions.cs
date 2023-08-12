using System.Diagnostics.CodeAnalysis;

namespace Hrim.Event.Analytics.JobWorker.Configuration;

[ExcludeFromCodeCoverage]
public record HrimRecurringJobOptions(string CronExpression,
                                  string DisplayName,
                                  string JobId,
                                  string Queue);