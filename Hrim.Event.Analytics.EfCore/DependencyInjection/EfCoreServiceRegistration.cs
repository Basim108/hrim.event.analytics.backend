using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.Entity;
using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Cqrs.Features;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.EfCore.AutoMapper;
using Hrim.Event.Analytics.EfCore.Cqrs.Entity;
using Hrim.Event.Analytics.EfCore.Cqrs.Events;
using Hrim.Event.Analytics.EfCore.Cqrs.EventTypes;
using Hrim.Event.Analytics.EfCore.Cqrs.Features;
using Hrimsoft.Data.PostgreSql;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hrim.Event.Analytics.EfCore.DependencyInjection;

public static class EfCoreServiceRegistration
{
    public static void AddEventAnalyticsStorage(this IServiceCollection services, IConfiguration appConfig, string migrationAssembly) {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<EventTypeCreateHandler>());
        services.AddTransient<IRequestHandler<SoftDeleteEntityCommand<UserEventType>, CqrsResult<UserEventType?>>, SoftDeleteEntityCommandHandler<UserEventType>>();
        services.AddTransient<IRequestHandler<SoftDeleteEntityCommand<OccurrenceEvent>, CqrsResult<OccurrenceEvent?>>, SoftDeleteEntityCommandHandler<OccurrenceEvent>>();
        services.AddTransient<IRequestHandler<SoftDeleteEntityCommand<DurationEvent>, CqrsResult<DurationEvent?>>, SoftDeleteEntityCommandHandler<DurationEvent>>();
        services.AddTransient<IRequestHandler<SoftDeleteEntityCommand<HrimTag>, CqrsResult<HrimTag?>>, SoftDeleteEntityCommandHandler<HrimTag>>();
        services.AddTransient<IRequestHandler<SoftDeleteEntityCommand<HrimUser>, CqrsResult<HrimUser?>>, SoftDeleteEntityCommandHandler<HrimUser>>();

        services.AddTransient<IRequestHandler<RestoreEntityCommand<UserEventType>, CqrsResult<UserEventType?>>, RestoreEntityCommandHandler<UserEventType>>();
        services.AddTransient<IRequestHandler<RestoreEntityCommand<OccurrenceEvent>, CqrsResult<OccurrenceEvent?>>, RestoreEntityCommandHandler<OccurrenceEvent>>();
        services.AddTransient<IRequestHandler<RestoreEntityCommand<DurationEvent>, CqrsResult<DurationEvent?>>, RestoreEntityCommandHandler<DurationEvent>>();
        services.AddTransient<IRequestHandler<RestoreEntityCommand<HrimTag>, CqrsResult<HrimTag?>>, RestoreEntityCommandHandler<HrimTag>>();
        services.AddTransient<IRequestHandler<RestoreEntityCommand<HrimUser>, CqrsResult<HrimUser?>>, RestoreEntityCommandHandler<HrimUser>>();

        services.AddTransient<IRequestHandler<GetEventById<DurationEvent>, CqrsResult<DurationEvent?>>, GetEventByIdHandler<DurationEvent>>();
        services.AddTransient<IRequestHandler<GetEventById<OccurrenceEvent>, CqrsResult<OccurrenceEvent?>>, GetEventByIdHandler<OccurrenceEvent>>();

        services.AddTransient<IRequestHandler<CheckUserExistence, CqrsVoidResult>, CheckEntityExistenceHandler>();
        services.AddTransient<IRequestHandler<CheckEventTypeExistence, CqrsVoidResult>, CheckEntityExistenceHandler>();
        services.AddTransient<IRequestHandler<SetupFeatures, Unit>, SetupFeaturesHandler>();

        services.AddAutoMapper(typeof(DbDurationEventProfile),
                               typeof(DbOccurrenceEventProfile));
        services.AddNpgsqlContext<EventAnalyticDbContext>(appConfig: appConfig, migrationAssembly: migrationAssembly);
    }
}