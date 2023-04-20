using Hrim.Event.Analytics.Api.Authentication;
using Hrimsoft.Core.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.DependencyInjection;

public static class AuthorizationRegistrations
{
    public static void AddEventAnalyticsAuthentication(this IServiceCollection services, IConfiguration appConfig) {
        services.AddScoped<EventAnalyticsOAuthEvents>();
        services.AddAuthentication(options => {
                     options.DefaultScheme          = CookieAuthenticationDefaults.AuthenticationScheme;
                     options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                 })
                .AddCookie(opt => { opt.Cookie.Name = "EventAnalytics"; })
                .AddGoogle(opt => {
                     var clientId = appConfig["GOOGLE_CLIENT_ID"];
                     if (string.IsNullOrWhiteSpace(clientId))
                         throw new ConfigurationException(null, "GOOGLE_CLIENT_ID");
                     var clientSecret = appConfig["GOOGLE_CLIENT_SECRET"];
                     if (string.IsNullOrWhiteSpace(clientSecret))
                         throw new ConfigurationException(null, "GOOGLE_CLIENT_SECRET");
                     opt.SignInScheme                   = CookieAuthenticationDefaults.AuthenticationScheme;
                     opt.ClientId                       = clientId;
                     opt.ClientSecret                   = clientSecret;
                     opt.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
                     opt.SaveTokens                     = true;
                     opt.ClaimActions.MapJsonKey(HrimClaims.PICTURE, "picture", "url");
                     opt.EventsType = typeof(EventAnalyticsOAuthEvents);
                 })
                .AddFacebook(opt => {
                     var appId = appConfig["FACEBOOK_APP_ID"];
                     if (string.IsNullOrWhiteSpace(appId))
                         throw new ConfigurationException(null, "FACEBOOK_APP_ID");
                     var appSecret = appConfig["FACEBOOK_APP_SECRET"];
                     if (string.IsNullOrWhiteSpace(appSecret))
                         throw new ConfigurationException(null, "FACEBOOK_APP_SECRET");
                     opt.SignInScheme                   = CookieAuthenticationDefaults.AuthenticationScheme;
                     opt.AppId                          = appId;
                     opt.AppSecret                      = appSecret;
                     opt.SaveTokens                     = true;
                     opt.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
                     opt.Fields.Add("picture");
                     opt.EventsType = typeof(EventAnalyticsOAuthEvents);
                 });
    }
}