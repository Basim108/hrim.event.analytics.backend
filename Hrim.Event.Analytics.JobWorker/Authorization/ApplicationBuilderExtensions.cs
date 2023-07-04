using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.JobWorker.Authorization;

public static class ApplicationBuilderExtensions
{
    public static void UseAnalyticsHangfireDashboard(this IApplicationBuilder app, IServiceProvider sp, IWebHostEnvironment env) {
        var options = new DashboardOptions();
        if (!env.IsDevelopment()) {
            options.Authorization = new[] {
                new HrimsoftDashboardAuthFilter(sp.GetRequiredService<IConfiguration>(),
                                                sp.GetRequiredService<ILogger<HrimsoftDashboardAuthFilter>>())
            };
        }
        app.UseHangfireDashboard("/jobs", options);
    }
}