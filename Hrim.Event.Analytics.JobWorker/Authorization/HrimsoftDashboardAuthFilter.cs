using System.Diagnostics.CodeAnalysis;
using Hangfire.Dashboard;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.JobWorker.Authorization;

[ExcludeFromCodeCoverage]
public class HrimsoftDashboardAuthFilter: IDashboardAuthorizationFilter
{
    private readonly IConfiguration                       _appConfig;
    private readonly ILogger<HrimsoftDashboardAuthFilter> _logger;

    public HrimsoftDashboardAuthFilter(IConfiguration                       appConfig,
                                       ILogger<HrimsoftDashboardAuthFilter> logger) {
        _appConfig = appConfig;
        _logger    = logger;
    }

    private readonly object _lockObj = new();

    public bool Authorize(DashboardContext context) {
        _logger.LogDebug(DashboardLogs.DASHBOARD_AUTH_CHECK_START);
        lock (_lockObj) {
            var isAuthorized = DoAuthorize(context);
            _logger.LogDebug(DashboardLogs.DASHBOARD_AUTH_CHECK_FINISH, isAuthorized);
            return isAuthorized;
        }
    }

    private bool DoAuthorize(DashboardContext context) {
        var user = context.GetHttpContext().User;
        if (!(user.Identity?.IsAuthenticated ?? false))
            return false;
        var userEmail = user.Claims.FirstOrDefault(x => x.Type.StartsWith("https://hrimsoft.us.auth0.com.example.com/email"))?.Value ?? "";
        if (string.IsNullOrWhiteSpace(userEmail)) {
            return false;
        }
        var adminEmail = _appConfig[DashboardLogs.HANGFIRE_ADMIN_EMAIL];
        if (string.IsNullOrWhiteSpace(adminEmail)) {
            _logger.LogCritical(DashboardLogs.ADMIN_EMAIL_IS_NOT_SET);
            return false;
        }
        var isAuthorized = userEmail == adminEmail;
        return isAuthorized;
    }
}