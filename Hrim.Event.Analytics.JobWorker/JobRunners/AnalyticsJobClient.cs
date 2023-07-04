using System.Diagnostics.CodeAnalysis;
using Hangfire;
using Hrim.Event.Analytics.Abstractions.Jobs;
using Hrim.Event.Analytics.JobWorker.Exceptions;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.JobWorker.JobRunners;

[ExcludeFromCodeCoverage]
public class AnalyticsJobClient: IAnalyticsJobClient
{
    private readonly ILogger<AnalyticsJobClient> _logger;
    private readonly IBackgroundJobClient        _hangfireJobClient;

    public AnalyticsJobClient(ILogger<AnalyticsJobClient> logger,
                              IBackgroundJobClient        hangfireJobClient) {
        _logger            = logger;
        _hangfireJobClient = hangfireJobClient;
    }

    /// <summary>
    /// Hope in the future hangfire will implement async methods to enqueue jobs.
    /// </summary>
    public Task<string> ScheduleAsync<TJob>(TJob job, DateTimeOffset runAt, CancellationToken cancellation)
        where TJob : IAnalyticsJob {
        if (runAt == default)
            throw new ArgumentNullException(nameof(runAt));
        var jobName = job.GetType().Name;
        try {
            var jobId = _hangfireJobClient.Schedule<MediatorHangfireBridge>(x => x.SendAsync(job), runAt);
            _logger.LogInformation(JobLogs.SCHEDULED_JOB, runAt, jobName, jobId);
            return Task.FromResult(jobId);
        }
        catch (Exception ex) {
            _logger.LogCritical(ex, JobLogs.ENQUEUEING_JOB_ERROR, jobName);
            throw new AnalyticsJobException(jobName, JobLogs.ENQUEUEING_JOB_ERROR, ex);
        }
    }

    /// <inheritdoc />
    public Task<string> EnqueueAsync<TJob>(TJob job, CancellationToken cancellation)
        where TJob : IAnalyticsJob {
        var jobName = job.GetType().Name;
        try {
            var jobId = _hangfireJobClient.Enqueue<MediatorHangfireBridge>(x => x.SendAsync(job));
            _logger.LogDebug(JobLogs.ENQUEUED_JOB, jobName, jobId);
            return Task.FromResult(jobId);
        }
        catch (Exception ex) {
            _logger.LogCritical(ex, JobLogs.ENQUEUEING_JOB_ERROR, jobName);
            throw new AnalyticsJobException(jobName, JobLogs.ENQUEUEING_JOB_ERROR, ex);
        }
    }
}