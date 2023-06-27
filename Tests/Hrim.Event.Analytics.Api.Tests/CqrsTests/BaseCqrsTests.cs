using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.Api.DependencyInjection;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;
using Hrim.Event.Analytics.EfCore;
using Hrim.Event.Analytics.EfCore.DependencyInjection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests;

/// <inheritdoc />
[ExcludeFromCodeCoverage]
public abstract class BaseCqrsTests: IDisposable
{
    protected IApiRequestAccessor ApiRequestAccessor { get; } = Substitute.For<IApiRequestAccessor>();

    private readonly IServiceScope _serviceScope;

    protected BaseCqrsTests() {
        var appConfig = new ConfigurationBuilder()
                       // .AddInMemoryCollection(new Dictionary<string, string>() { })
                       .AddJsonFile(path: "appsettings.Tests.json")
                       .Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddEventAnalyticsServices(appConfig: appConfig);
        services.AddEventAnalyticsStorage(appConfig: appConfig, typeof(Program).Assembly.GetName().Name!);

        services.CleanUpCurrentRegistrations(typeof(DbContextOptions<EventAnalyticDbContext>));
        services.AddDbContext<EventAnalyticDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        var operatorId = Guid.NewGuid();
        services.CleanUpCurrentRegistrations(typeof(IApiRequestAccessor));
        services.AddScoped(_ => {
            var claims = new List<Claim> {
                new(type: ClaimTypes.NameIdentifier, $"facebook|{UsersData.EXTERNAL_ID}"),
                new(type: "https://hrimsoft.us.auth0.com.example.com/email", value: UsersData.EMAIL)
            };
            var correlationId = Guid.NewGuid();
            ApiRequestAccessor.GetInternalUserIdAsync(Arg.Any<CancellationToken>()).Returns(returnThis: operatorId);
            ApiRequestAccessor.GetUserClaims().Returns(returnThis: claims);
            ApiRequestAccessor.GetCorrelationId().Returns(returnThis: correlationId);
            ApiRequestAccessor.GetOperationContext().Returns(new OperationContext(userClaims: claims, correlationId: correlationId));
            return ApiRequestAccessor;
        });
        _serviceScope   = services.BuildServiceProvider().CreateScope();
        ServiceProvider = _serviceScope.ServiceProvider;

        Mediator = ServiceProvider.GetRequiredService<IMediator>();
        var context = ServiceProvider.GetRequiredService<EventAnalyticDbContext>();
        TestData = new TestData(context: context);
        var apiRequestAccessor = ServiceProvider.GetRequiredService<IApiRequestAccessor>();
        OperatorContext = apiRequestAccessor.GetOperationContext();
        OperatorUserId  = operatorId;
        TestData.Users.EnsureUserExistence(id: OperatorUserId);
    }

    protected IMediator        Mediator        { get; }
    protected IServiceProvider ServiceProvider { get; }
    protected TestData         TestData        { get; }
    protected OperationContext OperatorContext { get; set; }

    protected Guid OperatorUserId { get; }

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (disposing) _serviceScope.Dispose();
    }
}