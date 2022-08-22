using FluentValidation;
using FluentValidation.AspNetCore;
using Hrim.Event.Analytics.Api.Extensions;
using Hrim.Event.Analytics.Api.Services;
using Hrim.Event.Analytics.Api.Swagger.Configuration;
using Hrim.Event.Analytics.EfCore.DependencyInjection;
using Hrim.Event.Analytics.Infrastructure.DependencyInjection;
using Hrimsoft.StringCases;

#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.DependencyInjection;

public static class ApiServiceCollectionRegistrations {
    public static void AddEventAnalyticsServices(this IServiceCollection services, IConfiguration appConfig) {
        services.AddControllers(options => options.UseDateOnlyTimeOnlyStringConverters())
                .AddHrimsoftJsonOptions();
        services.AddFluentValidationAutoValidation(_ => {
            ValidatorOptions.Global.LanguageManager.Enabled = false;
            ValidatorOptions.Global.DisplayNameResolver = (_, member, _)
                => member?.Name.ToSnakeCase();
            ValidatorOptions.Global.PropertyNameResolver = (_, member, _)
                => member?.Name.ToSnakeCase();
        });
        
        services.AddApiSwagger();
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        
        services.AddHttpContextAccessor();
        services.AddTransient<IApiRequestAccessor, ApiRequestAccessor>();
        
        services.AddEventAnalyticsInfrastructure();
        services.AddEventAnalyticsStorage(appConfig, typeof(Program).Assembly.GetName().Name!);
    }

}