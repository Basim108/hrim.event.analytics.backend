using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Hrim.Event.Analytics.EfCore;
using Hrimsoft.Core.Exceptions;
using Hrimsoft.Data.PostgreSql;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Hrim.Event.Analytics.JobWorker.Authorization;

public static class ServiceCollectionRegistrations
{
    public static void AddHangfireDashboardAuthorization(this IServiceCollection services, IConfiguration appConfig) {
        services.AddTransient<IDashboardAuthorizationFilter, HrimsoftDashboardAuthFilter>();
   
        services.AddAuthentication(options => {
                     options.DefaultScheme          = CookieAuthenticationDefaults.AuthenticationScheme;
                     options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                 })
                .AddCookie(options => {
                     options.Cookie.Name = "JobDashboard";
                 })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme,
                                  options => {
                                      var domain = appConfig[Envs.AUTH0_DOMAIN];
                                      if (string.IsNullOrWhiteSpace(domain))
                                          throw new ConfigurationException(null, Envs.AUTH0_DOMAIN);

                                      var clientId = appConfig[Envs.AUTH0_JOBS_CLIENT_ID];
                                      if (string.IsNullOrWhiteSpace(clientId))
                                          throw new ConfigurationException(null, Envs.AUTH0_JOBS_CLIENT_ID);

                                      var clientSecret = appConfig[Envs.AUTH0_JOBS_CLIENT_SECRET];
                                      if (string.IsNullOrWhiteSpace(clientSecret))
                                          throw new ConfigurationException(null, Envs.AUTH0_JOBS_CLIENT_SECRET);

                                      options.Authority                      = $"https://{domain}";
                                      options.ClientId                       = clientId;
                                      options.ClientSecret                   = clientSecret;
                                      options.CallbackPath                   = "/callback";
                                      options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
                                      options.NonceCookie.SecurePolicy       = CookieSecurePolicy.Always;
                                      options.SaveTokens                     = false;
                                      options.ResponseType                   = OpenIdConnectResponseType.Code;
                                      options.Scope.Clear();
                                      options.Scope.Add("openid");
                                      options.Scope.Add("profile");
                                      options.TokenValidationParameters = new TokenValidationParameters {
                                          NameClaimType = "name"
                                      };
                                      options.Events = new OpenIdConnectEvents {
                                          OnRedirectToIdentityProviderForSignOut = context => {
                                              var logoutUri     = $"https://{domain}/v2/logout?client_id={clientId}";
                                              var postLogoutUri = context.Properties.RedirectUri;
                                              if (!string.IsNullOrEmpty(postLogoutUri)) {
                                                  if (postLogoutUri.StartsWith("/")) {
                                                      // transform to absolute
                                                      var request = context.Request;
                                                      postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                                                  }
                                                  logoutUri = $"{logoutUri}&returnTo={Uri.EscapeDataString(postLogoutUri)}";
                                              }
                                              context.Response.Redirect(logoutUri);
                                              context.HandleResponse();

                                              return Task.CompletedTask;
                                          }
                                      };
                                  });
    }
}