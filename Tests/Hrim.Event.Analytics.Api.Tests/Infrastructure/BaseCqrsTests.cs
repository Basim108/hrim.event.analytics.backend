using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Api.DependencyInjection;
using Hrim.Event.Analytics.Api.Services;
using Hrim.Event.Analytics.EfCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure;

public class BaseCqrsTests {
    protected IMediator              Mediator        { get; }
    protected IConfiguration         AppConfig       { get; }
    protected IServiceProvider       ServiceProvider { get; }
    protected EventAnalyticDbContext DbContext       { get; }
    protected TestData               TestData        { get; }
    protected OperationContext       OperatorContext { get; }

    public BaseCqrsTests() {
        AppConfig = new ConfigurationBuilder()
                   // .AddInMemoryCollection(new Dictionary<string, string>() { })
                   .AddJsonFile("appsettings.Tests.json")
                   .Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddEventAnalyticsServices(AppConfig);

        services.CleanUpCurrentRegistrations(typeof(DbContextOptions<EventAnalyticDbContext>));
        services.AddDbContext<EventAnalyticDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        services.CleanUpCurrentRegistrations(typeof(IApiRequestAccessor));
        services.AddScoped(_ => {
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
        ServiceProvider = services.BuildServiceProvider().CreateScope().ServiceProvider;

        Mediator  = ServiceProvider.GetRequiredService<IMediator>();
        DbContext = ServiceProvider.GetRequiredService<EventAnalyticDbContext>();
        TestData  = new TestData(DbContext);
        var apiRequestAccessor = ServiceProvider.GetRequiredService<IApiRequestAccessor>();
        OperatorContext = apiRequestAccessor.GetOperationContext();

        TestData.CreateUser(OperatorContext.UserId);
    }
}