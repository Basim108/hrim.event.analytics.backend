using Hrim.Event.Analytics.EfCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure;

public class WebAppFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class {
    protected override void ConfigureWebHost(IWebHostBuilder builder) {
        builder.ConfigureAppConfiguration((context, configurationBuilder) => {
            configurationBuilder.AddJsonFile("appsettings.tests.json");
        });
        builder.ConfigureServices(services => {
            TestUtils.CleanUpCurrentRegistrations(services, typeof(DbContextOptions<EventAnalyticDbContext>));
            services.AddDbContext<EventAnalyticDbContext>(options => options.UseInMemoryDatabase("InMemoryDbForTesting"));
            
            var       sp    = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var       db    = scope.ServiceProvider.GetRequiredService<EventAnalyticDbContext>();
            
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        });
    }
}