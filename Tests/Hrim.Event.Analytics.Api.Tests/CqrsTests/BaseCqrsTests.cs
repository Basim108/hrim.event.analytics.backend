using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.Api.DependencyInjection;
using Hrim.Event.Analytics.Api.Services;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;
using Hrim.Event.Analytics.EfCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests;

/// <inheritdoc />
[ExcludeFromCodeCoverage]
public abstract class BaseCqrsTests : IDisposable
{
    private readonly   IServiceScope       _serviceScope;
    protected readonly IApiRequestAccessor _apiRequestAccessor = Substitute.For<IApiRequestAccessor>();
    protected BaseCqrsTests()
    {
        var appConfig = new ConfigurationBuilder()
            // .AddInMemoryCollection(new Dictionary<string, string>() { })
            .AddJsonFile("appsettings.Tests.json")
            .Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddEventAnalyticsServices(appConfig);

        services.CleanUpCurrentRegistrations(typeof(DbContextOptions<EventAnalyticDbContext>));
        services.AddDbContext<EventAnalyticDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        var operatorId = Guid.NewGuid();
        services.CleanUpCurrentRegistrations(typeof(IApiRequestAccessor));
        services.AddScoped(_ =>
        {
            var claims = new List<Claim> {
                new("sub", $"facebook|{UsersData.EXTERNAL_ID}"),
                new("https://hrimsoft.us.auth0.com.example.com/email", UsersData.EMAIL)
            };
            var correlationId = Guid.NewGuid();
            _apiRequestAccessor.GetInternalUserIdAsync(Arg.Any<CancellationToken>()).Returns(operatorId);
            _apiRequestAccessor.GetUserClaims().Returns(claims);
            _apiRequestAccessor.GetCorrelationId().Returns(correlationId);
            _apiRequestAccessor.GetOperationContext().Returns(new OperationContext(claims, correlationId));
            return _apiRequestAccessor;
        });
        _serviceScope = services.BuildServiceProvider().CreateScope();
        ServiceProvider = _serviceScope.ServiceProvider;

        Mediator = ServiceProvider.GetRequiredService<IMediator>();
        var context = ServiceProvider.GetRequiredService<EventAnalyticDbContext>();
        TestData = new TestData(context);
        var apiRequestAccessor = ServiceProvider.GetRequiredService<IApiRequestAccessor>();
        OperatorContext = apiRequestAccessor.GetOperationContext();
        OperatorUserId  = operatorId;
        TestData.Users.EnsureUserExistence(OperatorUserId);
    }

    protected IMediator        Mediator        { get; }
    protected IServiceProvider ServiceProvider { get; }
    protected TestData         TestData        { get; }
    protected OperationContext OperatorContext { get; set;  }
    
    protected Guid OperatorUserId { get; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing) _serviceScope.Dispose();
    }
}