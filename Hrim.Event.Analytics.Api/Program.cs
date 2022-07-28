using Hrim.Event.Analytics.Api.DependencyInjection;
using Hrim.Event.Analytics.Api.Swagger.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEventAnalyticsServices();
var app = builder.Build();

if(app.Environment.IsProduction()) {
    // app.UseExceptionHandler(c => c.Run(async httpContext => await ErrorHandler.OnExceptionAsync(httpContext)));
}else {
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        var cfg = SwaggerConfig.MakeEventAnalytics();
        c.SwaggerEndpoint($"{cfg.Version}/swagger.json", cfg.Title);
    });
}
app.Run();