using Hrim.Event.Analytics.Abstractions.Jobs.Configuration;

namespace Hrim.Event.Analytics.Abstractions.Jobs;

/// <summary> Interface that has to be used for enqueueing jobs </summary>
public interface IAnalyticsJobClient
{
    /// <summary> Enqueueing a job to run asap </summary>
    Task<string> EnqueueAsync<TJob>(TJob job, CancellationToken cancellation)
        where TJob : IAnalyticsJob;

    /// <summary> Enqueueing a job to run at a specific time </summary>
    Task<string> ScheduleAsync<TJob>(TJob job, DateTimeOffset runAt, CancellationToken cancellation)
        where TJob : IAnalyticsJob;

    /// <summary> Runs recurring jobs </summary>
    Task RunAsync<TRequest>(TRequest command, HrimRecurringJobOptions options, CancellationToken cancellation)
        where TRequest : AnalyticsRecurringJob;
}