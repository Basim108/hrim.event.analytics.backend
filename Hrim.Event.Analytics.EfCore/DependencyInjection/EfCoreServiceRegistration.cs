using Hrim.Event.Analytics.EfCore.Cqrs;
using Hrimsoft.Data.PostgreSql;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hrim.Event.Analytics.EfCore.DependencyInjection; 

public static class EfCoreServiceRegistration {
    public static void AddEventAnalyticsStorage(this IServiceCollection services, IConfiguration appConfig, string migrationAssembly) {
        services.AddMediatR(typeof(GetAllPublicViewEventTypesHandler).Assembly);
        services.AddNpgsqlContext<EventAnalyticDbContext>(appConfig, migrationAssembly);
    }
}