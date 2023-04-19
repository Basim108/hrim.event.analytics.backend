using Hrim.Event.Analytics.Api.DependencyInjection;
using Hrim.Event.Analytics.Api.Extensions;
using Hrim.Event.Analytics.EfCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddEventAnalyticsServices(builder.Configuration);
builder.Services.AddEventAnalyticsAuthentication(builder.Configuration);

var app = builder.Build();

app.UseEventAnalyticsCors(builder.Configuration);
app.UseHttpsRedirection();

app.UseSerilogRequestLogging();
app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    await next();
});
app.UseCorrelationId();
app.UseHttpContextLogging();
app.UseRouting();

app.MapHealthChecks("/health", new HealthCheckOptions
{
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
    {
    }
}