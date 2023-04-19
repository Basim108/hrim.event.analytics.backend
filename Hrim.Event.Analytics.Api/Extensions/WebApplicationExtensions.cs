using Hrim.Event.Analytics.Api.Swagger.Configuration;
using Hrimsoft.Core.Exceptions;

#pragma warning disable CS1591
namespace Hrim.Event.Analytics.Api.Extensions;

/// <summary>
///     Web application customizations
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary> Setups CORS </summary>
    public static void UseEventAnalyticsCors(this WebApplication app, IConfiguration appConfig)
    {
        var allowedOrigins = appConfig["ALLOWED_ORIGINS"];
        if (string.IsNullOrEmpty(allowedOrigins))
            throw new ConfigurationException(null, "ALLOWED_ORIGINS");
        var origins = allowedOrigins.Split(";", StringSplitOptions.RemoveEmptyEntries);
        app.UseCors(x =>
                        x.WithOrigins(origins)
                         .WithMethods("POST", "PUT", "GET", "DELETE", "PATCH")
                         .AllowCredentials()
                         .AllowAnyHeader());
    }

    /// <summary> Setup swagger </summary>
    public static void UseEventAnalyticsSwagger(this WebApplication app)
    {
        if (app.Environment.IsProduction())
            return;
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            var cfg = SwaggerConfig.MakeEventAnalytics();
            c.SwaggerEndpoint($"{cfg.Version}/swagger.json", cfg.Title);
        });
    }
}