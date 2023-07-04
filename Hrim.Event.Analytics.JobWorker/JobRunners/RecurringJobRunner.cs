using System.Diagnostics.CodeAnalysis;
using Hangfire;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs.Analysis;
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
        _logger               = logger;
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

    public static void SetupGapAnalysisJob(IServiceProvider sp) {
        var gapCfg = new GapHrimRecurringJobOptions();
        RecurringJob.RemoveIfExists(gapCfg.JobId);
        RecurringJob.AddOrUpdate(gapCfg.JobId,
                                 () => sp.GetRequiredService<RecurringJobRunner>()
                                         .RunAsync(new GapAnalysisRecurringJob(Guid.NewGuid()),
                                                   gapCfg.JobId,
                                                   CancellationToken.None),
                                 gapCfg.CronExpression,
                                 new RecurringJobOptions {
                                     TimeZone = TimeZoneInfo.Utc
                                 });
    }
}