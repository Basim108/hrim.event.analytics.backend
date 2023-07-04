using Hrim.Event.Analytics.Abstractions.Cqrs.Analysis;
using Hrim.Event.Analytics.JobWorker.Configuration;

namespace Hrim.Event.Analytics.JobWorker.JobRunners;

public static class RecurringJobRunnerExt
{
    public static Task RunGapAnalysis(this RecurringJobRunner runner, GapHrimRecurringJobOptions options)
        => runner.RunAsync(new GapAnalysisRecurringJob(Guid.NewGuid()),
                           options.JobId,
                           CancellationToken.None);
}