using FluentValidation;
using FluentValidation.AspNetCore;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.Api.Extensions;
using Hrim.Event.Analytics.Api.Services;
using Hrim.Event.Analytics.Api.Swagger.Configuration;
using Hrim.Event.Analytics.Api.V1.Validators.Entities.Events;
using Hrim.Event.Analytics.Api.V1.Validators.Entities.EventTypes;
using Hrim.Event.Analytics.EfCore.DependencyInjection;
using Hrim.Event.Analytics.Infrastructure.DependencyInjection;
using Hrimsoft.StringCases;

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

        services.AddEventAnalyticsInfrastructure();
        services.AddEventAnalyticsStorage(appConfig: appConfig, typeof(Program).Assembly.GetName().Name!);

        services.AddHealthChecks();
    }
}