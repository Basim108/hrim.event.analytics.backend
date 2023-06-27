using System.Diagnostics.CodeAnalysis;
using Hangfire;
using Hrim.Event.Analytics.Abstractions.Jobs;
using Hrim.Event.Analytics.Abstractions.Jobs.Configuration;
using Hrim.Event.Analytics.JobWorker.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.JobWorker.MediatR;

[ExcludeFromCodeCoverage]
public class AnalyticsJobRunner: IAnalyticsJobClient
{
    private readonly ILogger<AnalyticsJobRunner> _logger;
    private readonly IMediator                   _mediator;
    private readonly IBackgroundJobClient        _hangfireJobClient;

    public AnalyticsJobRunner(ILogger<AnalyticsJobRunner> logger,
                              IMediator                   mediator,
                              IBackgroundJobClient        hangfireJobClient) {
        _logger            = logger;
        _mediator          = mediator;
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

    /// <inheritdoc />
    public async Task RunAsync(IRequest                command,
                               HrimRecurringJobOptions options,
                               CancellationToken       cancellation) {
        using var jobIdScope = _logger.BeginScope(JobLogs.JOB_ID, options.JobId);
        try {
            await _mediator.Send(command, cancellation);
        }
        catch (Exception ex) {
            _logger.LogError(ex, JobLogs.RECURRING_JOB_FAILED_WITH_ERROR, options.JobId);
        }
    }
}