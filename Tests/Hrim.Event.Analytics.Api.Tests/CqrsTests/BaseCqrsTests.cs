using System.Diagnostics.CodeAnalysis;
using Hrim.Event.Analytics.Abstractions.Cqrs;
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
    private readonly IServiceScope _serviceScope;

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
        services.AddDbContext<EventAnalyticDbContext>(options =>
            options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        services.CleanUpCurrentRegistrations(typeof(IApiRequestAccessor));
        services.AddScoped(_ =>
        {
            var apiRequestAccessor = Substitute.For<IApiRequestAccessor>();
            var operatorId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();
            apiRequestAccessor.GetAuthorizedUserId()
                .Returns(operatorId);
            apiRequestAccessor.GetCorrelationId()
                .Returns(correlationId);
            apiRequestAccessor.GetOperationContext()
                .Returns(new OperationContext(operatorId, correlationId));
            return apiRequestAccessor;
        });
        _serviceScope = services.BuildServiceProvider().CreateScope();
        ServiceProvider = _serviceScope.ServiceProvider;

        Mediator = ServiceProvider.GetRequiredService<IMediator>();
        var context = ServiceProvider.GetRequiredService<EventAnalyticDbContext>();
        TestData = new TestData(context);
        var apiRequestAccessor = ServiceProvider.GetRequiredService<IApiRequestAccessor>();
        OperatorContext = apiRequestAccessor.GetOperationContext();

        TestData.Users.EnsureUserExistence(OperatorContext.UserId);
    }

    protected IMediator Mediator { get; }
    protected IServiceProvider ServiceProvider { get; }
    protected TestData TestData { get; }
    protected OperationContext OperatorContext { get; }

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