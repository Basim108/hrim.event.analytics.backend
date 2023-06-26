namespace Hrim.Event.Analytics.JobWorker;

public static class DashboardLogs
{
    public const string HANGFIRE_ADMIN_EMAIL = "HANGFIRE_ADMIN_EMAIL";

    public const string DASHBOARD_AUTH_CHECK_START  = "Hangfire dashboard authorization check is started";
    public const string DASHBOARD_AUTH_CHECK_FINISH = "Hangfire dashboard authorization check is finished IsAuthorized={IsAuthorized}";
    public const string ADMIN_EMAIL_IS_NOT_SET    = $"Hangfire dashboard is misconfigured; environment variable ${HANGFIRE_ADMIN_EMAIL} is empty";
}