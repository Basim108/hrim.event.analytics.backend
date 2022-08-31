using Hrim.Event.Analytics.Api.DependencyInjection;
using Hrim.Event.Analytics.Api.Extensions;
using Hrim.Event.Analytics.EfCore;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console()
                                       .ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddEventAnalyticsServices(builder.Configuration);
builder.Services.AddEventAnalyticsAuthentication(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseEventAnalyticsCors(builder.Configuration);

app.UseSerilogRequestLogging();
app.Use(async (context, next) => {
    context.Request.EnableBuffering();
    await next();
});
app.UseCorrelationId();
app.UseHttpContextLogging();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEventAnalyticsSwagger();

var dbContext = app.Services.CreateScope().ServiceProvider.GetRequiredService<EventAnalyticDbContext>();
if (dbContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory") {
    await dbContext.Database.MigrateAsync();
}

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
app.Run();

/// <summary> for integration tests </summary>
public partial class Program { }