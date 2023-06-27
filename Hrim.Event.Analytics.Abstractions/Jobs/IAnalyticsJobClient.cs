using Hrim.Event.Analytics.Abstractions.Jobs.Configuration;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Jobs;

public interface IAnalyticsJobClient
{
    /// <summary> Enqueueing a job to run asap </summary>
    Task<string> EnqueueAsync<TJob>(TJob job, CancellationToken cancellation)
        where TJob : IAnalyticsJob;

    /// <summary> Enqueueing a job to run at a specific time </summary>
    Task<string> ScheduleAsync<TJob>(TJob job, DateTimeOffset runAt, CancellationToken cancellation)
        where TJob : IAnalyticsJob;

    /// <summary> Runs recurring jobs </summary>
    Task RunAsync(IRequest command, HrimRecurringJobOptions options, CancellationToken cancellation);
}