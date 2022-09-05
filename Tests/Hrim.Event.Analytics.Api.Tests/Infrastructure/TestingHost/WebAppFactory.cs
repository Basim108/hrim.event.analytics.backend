using System.Diagnostics.CodeAnalysis;
using Hrim.Event.Analytics.EfCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure.TestingHost;

[ExcludeFromCodeCoverage]
public class WebAppFactory<TProgram>: WebApplicationFactory<TProgram> where TProgram : class {
    protected override void ConfigureWebHost(IWebHostBuilder builder) {
        builder.ConfigureAppConfiguration((_, configurationBuilder) => {
            configurationBuilder.AddJsonFile("appsettings.Tests.json");
        });
        builder.ConfigureServices(services => {
            services.AddAuthentication("IntegrationTest")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("IntegrationTest", _ => { });

            services.CleanUpCurrentRegistrations(typeof(DbContextOptions<EventAnalyticDbContext>));
            services.AddDbContext<EventAnalyticDbContext>(options => options.UseInMemoryDatabase("InMemoryDbForTesting"));

            var       sp    = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var       db    = scope.ServiceProvider.GetRequiredService<EventAnalyticDbContext>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var schemeProvider = scope.ServiceProvider.GetService<IAuthenticationSchemeProvider>();
            if (schemeProvider != null) {
                schemeProvider.RemoveScheme("Google");
                schemeProvider.RemoveScheme("Facebook");
            }
        });
    }
}