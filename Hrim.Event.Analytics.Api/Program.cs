using Hrim.Event.Analytics.Api.DependencyInjection;
using Hrim.Event.Analytics.Api.Extensions;
using Hrim.Event.Analytics.Api.Swagger.Configuration;
using Hrim.Event.Analytics.EfCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEventAnalyticsServices(builder.Configuration);
var app = builder.Build();

app.UseCorrelationId();
app.UseRouting();

if (!app.Environment.IsProduction()){
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        var cfg = SwaggerConfig.MakeEventAnalytics();
        c.SwaggerEndpoint($"{cfg.Version}/swagger.json", cfg.Title);
    });
}
var dbContext = app.Services.CreateScope().ServiceProvider.GetRequiredService<EventAnalyticDbContext>();
await dbContext.Database.MigrateAsync();

app.UseCors(b => b.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader());
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.Run();