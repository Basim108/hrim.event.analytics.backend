using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.EfCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure.TestingHost;

[ExcludeFromCodeCoverage]
public class WebAppFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            configurationBuilder.AddJsonFile("appsettings.Tests.json");
        });
        builder.ConfigureServices(services =>
        {
            services.AddAuthentication("IntegrationTest")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("IntegrationTest", _ => { });

            services.CleanUpCurrentRegistrations(typeof(DbContextOptions<EventAnalyticDbContext>));
            services.AddDbContext<EventAnalyticDbContext>(options => options.UseInMemoryDatabase("InMemoryDbForTesting"));

            var operatorId = Guid.NewGuid();
            services.CleanUpCurrentRegistrations(typeof(IApiRequestAccessor));
            services.AddScoped(_ =>
            {
                var apiRequestAccessor = Substitute.For<IApiRequestAccessor>();
                var claims = new List<Claim> {
                    new("sub", $"facebook|{UsersData.EXTERNAL_ID}"),
                    new("https://hrimsoft.us.auth0.com.example.com/email", UsersData.EMAIL)
                };
                var correlationId = Guid.NewGuid();
                apiRequestAccessor.GetInternalUserIdAsync(Arg.Any<CancellationToken>()).Returns(operatorId);
                apiRequestAccessor.GetUserClaims().Returns(claims);
                apiRequestAccessor.GetCorrelationId().Returns(correlationId);
                apiRequestAccessor.GetOperationContext().Returns(new OperationContext(claims, correlationId));
                return apiRequestAccessor;
            });
            
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<EventAnalyticDbContext>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        });
    }
}