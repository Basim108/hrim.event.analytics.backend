using System.Reflection;
using Hrim.Event.Analytics.Models;
using Hrim.Event.Analytics.Models.Entities;
using Swashbuckle.AspNetCore.Filters;

#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.Swagger.Configuration;

public static class SwaggerServiceCollectionRegistrations {
    public static void AddApiSwagger(this IServiceCollection services) {
        services.AddSwaggerGenNewtonsoftSupport();
        services.AddSwaggerGen(c => {
            c.SwaggerDoc("v1", SwaggerConfig.MakeEventAnalytics());
            c.ExampleFilters();
            c.IncludeXmlComments(GetXmlCommentsPath());
            c.IncludeXmlComments(GetModelXmlCommentsPath());
            c.OperationFilter<AddResponseHeadersFilter>();
            c.UseAllOfForInheritance();
            c.UseOneOfForPolymorphism();
        });
        services.AddSwaggerExamplesFromAssemblyOf<Program>();
    }

    private static string GetXmlCommentsPath() {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        return xmlPath;
    }

    private static string GetModelXmlCommentsPath() {
        var xmlFile = $"{Assembly.GetAssembly(typeof(Entity))?.GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        return xmlPath;
    }
}