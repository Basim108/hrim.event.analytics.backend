using Hrim.Event.Analytics.Api.Authentication;
using Hrimsoft.Core.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.DependencyInjection;

public static class AuthorizationRegistrations
{
    public static void AddEventAnalyticsAuthentication(this IServiceCollection services, IConfiguration appConfig) {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                 {
                     var domain = appConfig["AUTH0_DOMAIN"];
                     if (string.IsNullOrWhiteSpace(domain))
                         throw new ConfigurationException(null, "AUTH0_DOMAIN");

                     options.Authority = $"https://{domain}/";
                     options.Audience  = "event-analytics-crud-api";
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateAudience         = true,
                         ValidateIssuerSigningKey = true
                     };
                 });
    }
}