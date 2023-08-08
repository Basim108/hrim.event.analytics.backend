using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.Analysis.Cqrs.CountAnalysis;
using Hrim.Event.Analytics.Analysis.Cqrs.GapAnalysis;
using Hrim.Event.Analytics.Analysis.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hrim.Event.Analytics.Analysis.DependencyInjection;

public static class AnalysisServiceRegistrations
{
    public static void AddEventAnalyticsAnalysisServices(this IServiceCollection services) {
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(assembly: typeof(AnalysisLogs).Assembly);
        });

        services.AddTransient<IGapCalculationService, GapCalculationService>();
        services.AddTransient<ICountCalculationService, CountCalculationService>();
        services.AddTransient<IAnalysisSettingsFactory, AnalysisSettingsFactory>();
    }
}