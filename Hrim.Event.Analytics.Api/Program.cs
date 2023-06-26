using Hangfire;
using Hrim.Event.Analytics.Abstractions.Cqrs.Features;
using Hrim.Event.Analytics.Api.DependencyInjection;
using Hrim.Event.Analytics.Api.Extensions;
using Hrim.Event.Analytics.EfCore;
using Hrim.Event.Analytics.EfCore.DependencyInjection;
using Hrim.Event.Analytics.JobWorker.Authorization;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args: args);

builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(assembly: typeof(Program).Assembly);
});
builder.Services.AddEventAnalyticsServices(builder.Configuration);
builder.Services.AddEventAnalyticsStorage(builder.Configuration, typeof(Program).Assembly.GetName().Name!);
builder.Services.AddEventAnalyticsAuthentication(builder.Configuration);
builder.Services.AddEventAnalyticsHangfire(builder.Configuration);
builder.Services.AddHangfireDashboardAuthorization(builder.Configuration);

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
app.UseStaticFiles();
var sp = app.Services.CreateScope().ServiceProvider;

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
app.UseAnalyticsHangfireDashboard(sp, app.Environment);

if (!app.Environment.IsProduction())
    app.UseEventAnalyticsSwagger();

var mediator  = sp.GetRequiredService<IMediator>();
var dbContext = sp.GetRequiredService<EventAnalyticDbContext>();
if (dbContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
    await dbContext.Database.MigrateAsync();

await mediator.Send(new SetupFeatures());

app.MapControllers();
app.Run();

// add for sonar lint issue
namespace Hrim.Event.Analytics.Api
{
    /// <summary> for integration tests </summary>
    public class Program
    { }
}