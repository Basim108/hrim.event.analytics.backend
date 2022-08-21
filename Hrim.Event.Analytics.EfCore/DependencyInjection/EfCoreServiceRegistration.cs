using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
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
        services.AddMediatR(typeof(CreateEventTypeHandler).Assembly);
        services.AddTransient<IRequestHandler<SoftDeleteEntityCommand<SystemEventType>, CqrsResult<SystemEventType?>>, SoftDeleteEntityCommandHandler<SystemEventType>>();
        services.AddTransient<IRequestHandler<SoftDeleteEntityCommand<OccurrenceEvent>, CqrsResult<OccurrenceEvent?>>, SoftDeleteEntityCommandHandler<OccurrenceEvent>>();
        services.AddTransient<IRequestHandler<SoftDeleteEntityCommand<DurationEvent>, CqrsResult<DurationEvent?>>, SoftDeleteEntityCommandHandler<DurationEvent>>();
        services.AddTransient<IRequestHandler<SoftDeleteEntityCommand<HrimTag>, CqrsResult<HrimTag?>>, SoftDeleteEntityCommandHandler<HrimTag>>();
        services.AddTransient<IRequestHandler<SoftDeleteEntityCommand<HrimUser>, CqrsResult<HrimUser?>>, SoftDeleteEntityCommandHandler<HrimUser>>();

        services.AddTransient<IRequestHandler<RestoreEntityCommand<SystemEventType>, CqrsResult<SystemEventType?>>, RestoreEntityCommandHandler<SystemEventType>>();
        services.AddTransient<IRequestHandler<RestoreEntityCommand<OccurrenceEvent>, CqrsResult<OccurrenceEvent?>>, RestoreEntityCommandHandler<OccurrenceEvent>>();
        services.AddTransient<IRequestHandler<RestoreEntityCommand<DurationEvent>, CqrsResult<DurationEvent?>>, RestoreEntityCommandHandler<DurationEvent>>();
        services.AddTransient<IRequestHandler<RestoreEntityCommand<HrimTag>, CqrsResult<HrimTag?>>, RestoreEntityCommandHandler<HrimTag>>();
        services.AddTransient<IRequestHandler<RestoreEntityCommand<HrimUser>, CqrsResult<HrimUser?>>, RestoreEntityCommandHandler<HrimUser>>();

        
        services.AddAutoMapper(typeof(DurationEventTypeProfile),
                               typeof(OccurrenceEventTypeProfile));
        services.AddNpgsqlContext<EventAnalyticDbContext>(appConfig, migrationAssembly);
    }
}