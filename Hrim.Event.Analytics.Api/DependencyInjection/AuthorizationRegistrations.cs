using Hrimsoft.Core.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.DependencyInjection;

public static class AuthorizationRegistrations
{
    public static void AddEventAnalyticsAuthentication(this IServiceCollection services, IConfiguration appConfig) {
        services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                     var domain = appConfig[key: "AUTH0_DOMAIN"];
                     if (string.IsNullOrWhiteSpace(value: domain))
                         throw new ConfigurationException(sectionName: null, key: "AUTH0_DOMAIN");

                     options.Authority = $"https://{domain}/";
                     options.Audience  = "event-analytics-crud-api";
                     options.TokenValidationParameters = new TokenValidationParameters {
                         ValidateAudience         = true,
                         ValidateIssuerSigningKey = true
                     };
                 });
    }
}