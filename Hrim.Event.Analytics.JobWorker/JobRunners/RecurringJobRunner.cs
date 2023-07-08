using System.Diagnostics.CodeAnalysis;
using Hangfire;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Jobs;
using Hrim.Event.Analytics.JobWorker.Configuration;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.JobWorker.JobRunners;

[ExcludeFromCodeCoverage]
public class RecurringJobRunner
{
    private readonly ILogger<RecurringJobRunner> _logger;
    private readonly IServiceProvider            _serviceProvider;

    public RecurringJobRunner(ILogger<RecurringJobRunner> logger, IServiceProvider serviceProvider) {
        _logger          = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task RunAsync<TRequest>(TRequest command, string jobId, CancellationToken cancellationToken)
        where TRequest : AnalyticsRecurringJob {
        using var jobIdScope         = _logger.BeginScope(JobLogs.JOB_ID,          jobId);
        using var correlationIdScope = _logger.BeginScope(CoreLogs.CORRELATION_ID, command.CorrelationId);

        using var scope = _serviceProvider.CreateScope();
        try {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Send(command, cancellationToken);
        }
        catch (Exception ex) {
            _logger.LogError(ex, JobLogs.RECURRING_JOB_FAILED_WITH_ERROR, jobId);
            // suppress exceptions to avoid retries on recurring jobs 
        }
    }

    public static void SetupAnalysisJob<TJob, TOptions>(IServiceProvider sp)
        where TJob: AnalyticsRecurringJob, new()
        where TOptions: HrimRecurringJobOptions, new() 
    {
        var analysisJobCfg = new TOptions();
        RecurringJob.RemoveIfExists(analysisJobCfg.JobId);
        RecurringJob.AddOrUpdate(analysisJobCfg.JobId,
                                 () => sp.GetRequiredService<RecurringJobRunner>()
                                         .RunAsync(new TJob(), new TOptions().JobId, CancellationToken.None),
                                 analysisJobCfg.CronExpression,
                                 new RecurringJobOptions {
                                     TimeZone = TimeZoneInfo.Utc
                                 });
    }
}