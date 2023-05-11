using Hrim.Event.Analytics.Api.DependencyInjection;
using Hrim.Event.Analytics.Api.Extensions;
using Hrim.Event.Analytics.EfCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args: args);

builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console()
                                       .ReadFrom.Configuration(configuration: ctx.Configuration));

builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(assembly: typeof(Program).Assembly);
});
builder.Services.AddEventAnalyticsServices(appConfig: builder.Configuration);
builder.Services.AddEventAnalyticsAuthentication(appConfig: builder.Configuration);
builder.Services.Configure<ForwardedHeadersOptions>(options => {
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
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