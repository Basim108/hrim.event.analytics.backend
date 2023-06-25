using FluentValidation;
using FluentValidation.AspNetCore;
using Hrim.Event.Analytics.Abstractions.Cqrs.Features;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Features;
using Hrim.Event.Analytics.Api.Extensions;
using Hrim.Event.Analytics.Api.Services;
using Hrim.Event.Analytics.Api.Swagger.Configuration;
using Hrim.Event.Analytics.Api.V1.Validators.Entities.Analysis;
using Hrim.Event.Analytics.Api.V1.Validators.Entities.Events;
using Hrim.Event.Analytics.Api.V1.Validators.Entities.EventTypes;
using Hrim.Event.Analytics.EfCore.Cqrs.Analysis;
using Hrim.Event.Analytics.EfCore.DependencyInjection;
using Hrim.Event.Analytics.Infrastructure.DependencyInjection;
using Hrimsoft.StringCases;
using MediatR;

#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.DependencyInjection;

public static class ApiServiceCollectionRegistrations
{
    public static void AddEventAnalyticsServices(this IServiceCollection services, IConfiguration appConfig) {
        services.AddCors();
        services.AddControllers()
                .AddHrimsoftJsonOptions();
        services.AddFluentValidationAutoValidation(_ => {
            ValidatorOptions.Global.LanguageManager.Enabled = false;
            ValidatorOptions.Global.DisplayNameResolver = (_, member, _)
                => member?.Name.ToSnakeCase();
            ValidatorOptions.Global.PropertyNameResolver = (_, member, _)
                => member?.Name.ToSnakeCase();
        });
        services.AddValidatorsFromAssembly(assembly: typeof(Program).Assembly);
        services.AddApiSwagger();
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

        services.AddHttpContextAccessor();
        services.AddScoped<IApiRequestAccessor, ApiRequestAccessor>();
        services.AddTransient<IValidator<DurationEvent>, EventAsyncValidator>();
        services.AddTransient<IValidator<OccurrenceEvent>, EventAsyncValidator>();
        services.AddTransient<IValidator<UserEventType>, EventTypeAsyncValidator>();
        services.AddTransient<IValidator<AnalysisByEventType>, AnalysisByEventTypeValidator>();
        services.AddTransient<IValidator<List<AnalysisByEventType>>, AnalysisByEventTypeListValidator>();
        
        services.AddTransient<IRequestHandler<GetAvailableAnalysisQuery, List<AvailableAnalysis>>, GetAvailableAnalysisQueryHandler>();

        services.AddEventAnalyticsInfrastructure();
        services.AddEventAnalyticsStorage(appConfig: appConfig, typeof(Program).Assembly.GetName().Name!);

        services.AddHealthChecks();
    }
}