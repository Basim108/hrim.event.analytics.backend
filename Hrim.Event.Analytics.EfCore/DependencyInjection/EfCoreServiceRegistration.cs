using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.EfCore.AutoMapper;
using Hrim.Event.Analytics.EfCore.Cqrs;
using Hrim.Event.Analytics.EfCore.Cqrs.EventTypes;
using Hrimsoft.Data.PostgreSql;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hrim.Event.Analytics.EfCore.DependencyInjection;

public static class EfCoreServiceRegistration {
    public static void AddEventAnalyticsStorage(this IServiceCollection services, IConfiguration appConfig, string migrationAssembly) {
        services.AddMediatR(typeof(GetViewEventTypesHandler).Assembly);
        services.AddTransient<IRequestHandler<SoftDeleteEntityCommand<OccurrenceEventType>, CqrsResult<OccurrenceEventType?>>, SoftDeleteEntityCommandHandler<OccurrenceEventType>>();
        services.AddTransient<IRequestHandler<SoftDeleteEntityCommand<DurationEventType>, CqrsResult<DurationEventType?>>, SoftDeleteEntityCommandHandler<DurationEventType>>();
        services.AddTransient<IRequestHandler<SoftDeleteEntityCommand<HrimTag>, CqrsResult<HrimTag?>>, SoftDeleteEntityCommandHandler<HrimTag>>();
        services.AddTransient<IRequestHandler<SoftDeleteEntityCommand<HrimUser>, CqrsResult<HrimUser?>>, SoftDeleteEntityCommandHandler<HrimUser>>();

        services.AddAutoMapper(typeof(DurationEventTypeProfile),
                               typeof(OccurrenceEventTypeProfile));
        services.AddNpgsqlContext<EventAnalyticDbContext>(appConfig, migrationAssembly);
    }
}