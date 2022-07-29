using Hrim.Event.Analytics.Api.DependencyInjection;
using Hrim.Event.Analytics.Api.Extensions;
using Hrim.Event.Analytics.Api.Swagger.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEventAnalyticsServices();
var app = builder.Build();

app.UseCorrelationId();
app.UseRouting();

if(app.Environment.IsProduction()) {
}else {
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        var cfg = SwaggerConfig.MakeEventAnalytics();
        c.SwaggerEndpoint($"{cfg.Version}/swagger.json", cfg.Title);
    });
}
app.UseCors(b => b.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader());
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.Run();