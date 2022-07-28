using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hrim.Event.Analytics.Api.Extensions;
using Hrim.Event.Analytics.Api.Swagger.Configuration;
using Hrim.Event.Analytics.Models;
using Hrimsoft.StringCases;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.DependencyInjection;

public static class ApiServiceCollectionRegistrations {
    public static void AddEventAnalyticsServices(this IServiceCollection services) {
        services.AddControllers()
                .AddHrimsoftJsonOptions()
                .AddFluentValidation(_ => {
                     ValidatorOptions.Global.LanguageManager.Enabled = false;
                     ValidatorOptions.Global.DisplayNameResolver = (_, member, _)
                         => member?.Name.ToSnakeCase();
                     ValidatorOptions.Global.PropertyNameResolver = (_, member, _)
                         => member?.Name.ToSnakeCase();
                 });
        services.AddApiSwagger();
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    }

    public static void AddApiSwagger(this IServiceCollection services) {
        services.AddSwaggerGenNewtonsoftSupport();
        services.AddSwaggerGen(c => {
            c.SwaggerDoc("v1", SwaggerConfig.MakeEventAnalytics());
            c.ExampleFilters();
            c.IncludeXmlComments(GetXmlCommentsPath());
            c.IncludeXmlComments(GetModelXmlCommentsPath());
            c.OperationFilter<AddResponseHeadersFilter>();
        });
        services.AddSwaggerExamplesFromAssemblyOf<Program>();
    }

    private static string GetXmlCommentsPath() {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        return xmlPath;
    }

    private static string GetModelXmlCommentsPath() {
        var xmlFile = $"{Assembly.GetAssembly(typeof(Class1))?.GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        return xmlPath;
    }
}