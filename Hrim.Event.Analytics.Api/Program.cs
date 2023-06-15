using Hrim.Event.Analytics.Api.DependencyInjection;
using Hrim.Event.Analytics.Api.Extensions;
using Hrim.Event.Analytics.EfCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args: args);

builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(assembly: typeof(Program).Assembly);
});
builder.Services.AddEventAnalyticsServices(appConfig: builder.Configuration);
builder.Services.AddEventAnalyticsAuthentication(appConfig: builder.Configuration);
builder.Services.Configure<ForwardedHeadersOptions>(options => {
    options.ForwardedHeaders = ForwardedHeaders.All;
});
builder.Host.UseSerilog((context, services, configuration) => {
    var loggerCfg = configuration.ReadFrom.Configuration(context.Configuration);
    if (context.HostingEnvironment.IsDevelopment()) {
        loggerCfg.MinimumLevel.Verbose();
    }
    else {
        loggerCfg.ReadFrom.Services(services)
                 .Enrich.WithProperty("ThreadId", Environment.CurrentManagedThreadId)
                 .Enrich.WithProperty("AspNetEnvironment", context.HostingEnvironment.EnvironmentName);
    }
    if (context.HostingEnvironment.IsDevelopment()) {
        configuration.WriteTo.Console();
    }
    else {
        configuration.WriteTo.Console(new RenderedCompactJsonFormatter());
    }
});
var app = builder.Build();

app.UseForwardedHeaders();
app.UseEventAnalyticsCors(appConfig: builder.Configuration);

app.UseSerilogRequestLogging();

app.Use(async (context, next) => {
    context.Request.EnableBuffering();
    await next();
});
app.UseCorrelationId();
app.UseHttpContextLogging();
app.UseRouting();

app.MapHealthChecks(pattern: "/health",
                    new HealthCheckOptions {
                        AllowCachingResponses = false
                    });

app.UseAuthentication();
app.UseAuthorization();

if (!app.Environment.IsProduction())
    app.UseEventAnalyticsSwagger();

var dbContext = app.Services.CreateScope().ServiceProvider.GetRequiredService<EventAnalyticDbContext>();
if (dbContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
    await dbContext.Database.MigrateAsync();

app.MapControllers();
app.Run();

// add for sonar lint issue
namespace Hrim.Event.Analytics.Api
{
    /// <summary> for integration tests </summary>
    public class Program
    { }
}