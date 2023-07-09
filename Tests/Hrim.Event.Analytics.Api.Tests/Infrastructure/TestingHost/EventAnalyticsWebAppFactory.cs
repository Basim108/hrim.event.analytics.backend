using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.EfCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure.TestingHost;

[ExcludeFromCodeCoverage]
public class EventAnalyticsWebAppFactory<TProgram>: WebApplicationFactory<TProgram>, IAsyncLifetime, IDisposable
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder) {
        builder.ConfigureAppConfiguration((_, configurationBuilder) => {
            configurationBuilder.AddJsonFile(path: "appsettings.Tests.json", optional: false, reloadOnChange: false);
        });
        builder.ConfigureTestServices(services => {
            services.Replace(new ServiceDescriptor(typeof(IPolicyEvaluator), typeof(DisableAuthenticationPolicyEvaluator), ServiceLifetime.Singleton));
        });
        builder.ConfigureServices(services => {
            services.CleanUpCurrentRegistrations(typeof(DbContextOptions<EventAnalyticDbContext>));
            services.AddDbContext<EventAnalyticDbContext>(options => options.UseInMemoryDatabase(databaseName: "InMemoryDbForTesting"));

            var operatorId = Guid.NewGuid();
            services.CleanUpCurrentRegistrations(typeof(IApiRequestAccessor));
            services.AddScoped(_ => {
                var apiRequestAccessor = Substitute.For<IApiRequestAccessor>();
                var claims = new List<Claim> {
                    new(type: ClaimTypes.NameIdentifier, $"facebook|{UsersData.EXTERNAL_ID}"),
                    new(type: "https://hrimsoft.us.auth0.com.example.com/email", value: UsersData.EMAIL)
                };
                var correlationId = Guid.NewGuid();
                apiRequestAccessor.GetInternalUserIdAsync(Arg.Any<CancellationToken>()).Returns(returnThis: operatorId);
                apiRequestAccessor.GetUserClaims().Returns(returnThis: claims);
                apiRequestAccessor.GetCorrelationId().Returns(returnThis: correlationId);
                apiRequestAccessor.GetOperationContext().Returns(new OperationContext(userClaims: claims, correlationId: correlationId));
                return apiRequestAccessor;
            });

            var       sp    = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var       db    = scope.ServiceProvider.GetRequiredService<EventAnalyticDbContext>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        });
    }

    public Task InitializeAsync() { return Task.CompletedTask; }

    public void Dispose() {
        try {
            base.Dispose();
        }
        catch (Exception ex) {
            if (!ex.ToString().Contains("Hangfire.Server.BackgroundProcessingServer"))
                throw new Exception(ex.Message, ex);
        }
    }

    public async Task DisposeAsync() {
        try {
            await base.DisposeAsync();
        }
        catch (Exception ex) {
            if (!ex.ToString().Contains("Hangfire.Server.BackgroundProcessingServer"))
                throw new Exception(ex.Message, ex);
        }
    }
}