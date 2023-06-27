namespace Hrim.Event.Analytics.JobWorker;

public static class JobLogs
{
    public const string JOB_ID        = "EventAnalyticsJobId={EventAnalyticsJobId}";
    public const string ENQUEUED_JOB  = "Enqueued job EventAnalyticsJob={EventAnalyticsJob} EventAnalyticsJobId={EventAnalyticsJobId}";
    public const string SCHEDULED_JOB = "Scheduled job RunJobAt={RunJobAt} EventAnalyticsJob={EventAnalyticsJob} EventAnalyticsJobId={EventAnalyticsJobId}";

    public const string ENQUEUEING_JOB_ERROR            = "Failed to enqueue a job EventAnalyticsJob={EventAnalyticsJob}";
    public const string JOB_FAILED_WITH_ERROR           = "Job failed with error CqrsCommand={CqrsCommand}";
    public const string RECURRING_JOB_FAILED_WITH_ERROR = "Recurring job failed with error EventAnalyticsJobId={EventAnalyticsJobId}";
}