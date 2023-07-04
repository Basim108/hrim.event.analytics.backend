using Hangfire;
using Hangfire.PostgreSql;
using Hrim.Event.Analytics.Abstractions.Jobs;
using Hrim.Event.Analytics.JobWorker.JobRunners;
using Hrimsoft.Data.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hrim.Event.Analytics.JobWorker.DependencyInjection;

public static class JobWorkerRegistrations
{
    public static void AddEventAnalyticsJobWorker(this IServiceCollection services) {
        services.AddTransient<RecurringJobRunner>();
        services.AddTransient<IAnalyticsJobClient, AnalyticsJobClient>();
    }
    
    public static void AddEventAnalyticsHangfireServer(this IServiceCollection services, IConfiguration appConfig) {
        services.AddHangfire((sp, configuration) => {
            configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                         .UseSimpleAssemblyNameTypeSerializer()
                         .UseRecommendedSerializerSettings();
            configuration.UseActivator(new ServiceProviderJobActivator(sp));
            var isIntegrationTesting = !string.IsNullOrWhiteSpace(appConfig["INTEGRATION_TESTING"]);
            if (isIntegrationTesting) {
                GlobalConfiguration.Configuration.UseInMemoryStorage();
            }
            else {
                var (connectionString, _, _) = ConnectionStringBuilder.Get(appConfig);
                configuration.UsePostgreSqlStorage(connectionString,
                                                   new PostgreSqlStorageOptions { SchemaName = "hangfire" });
            }
            configuration.UseSerilogLogProvider();
        });
        services.AddHangfireServer(options => {
            options.Queues = new[] {
                Queues.ANALYSIS,
                Queues.DEFAULT,
                Queues.NOTIFICATIONS
            };
        });
    }
}