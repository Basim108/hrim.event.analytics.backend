using System.Reflection;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Swashbuckle.AspNetCore.Filters;

#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.Swagger.Configuration;

public static class SwaggerServiceCollectionRegistrations
{
    public static void AddApiSwagger(this IServiceCollection services) {
        services.AddSwaggerGenNewtonsoftSupport();
        services.AddSwaggerGen(c => {
            c.UseDateOnlyTimeOnlyStringConverters();
            c.SwaggerDoc(name: "v1", SwaggerConfig.MakeEventAnalytics());
            c.ExampleFilters();
            c.IncludeXmlComments(GetXmlCommentsPath());
            c.IncludeXmlComments(GetAbstractionsXmlCommentsPath());
            c.OperationFilter<AddResponseHeadersFilter>();
            c.UseAllOfForInheritance();
            c.UseOneOfForPolymorphism();
        });
        services.AddSwaggerExamplesFromAssemblyOf<Program>();
    }

    private static string GetXmlCommentsPath() {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(path1: AppContext.BaseDirectory, path2: xmlFile);
        return xmlPath;
    }

    private static string GetAbstractionsXmlCommentsPath() {
        var xmlFile = $"{Assembly.GetAssembly(typeof(BaseEvent))?.GetName().Name}.xml";
        var xmlPath = Path.Combine(path1: AppContext.BaseDirectory, path2: xmlFile);
        return xmlPath;
    }
}