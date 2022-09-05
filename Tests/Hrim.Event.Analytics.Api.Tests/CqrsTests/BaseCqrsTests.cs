using AutoMapper;
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

public class BaseCqrsTests {
    protected IMapper                Mapper          { get; }
    protected IMediator              Mediator        { get; }
    protected IServiceProvider       ServiceProvider { get; }
    protected EventAnalyticDbContext DbContext       { get; }
    protected TestData               TestData        { get; }
    protected OperationContext       OperatorContext { get; }

    public BaseCqrsTests() {
        var appConfig = new ConfigurationBuilder()
                        // .AddInMemoryCollection(new Dictionary<string, string>() { })
                       .AddJsonFile("appsettings.Tests.json")
                       .Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddEventAnalyticsServices(appConfig);

        services.CleanUpCurrentRegistrations(typeof(DbContextOptions<EventAnalyticDbContext>));
        services.AddDbContext<EventAnalyticDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        services.CleanUpCurrentRegistrations(typeof(IApiRequestAccessor));
        services.AddScoped(_ => {
            var apiRequestAccessor = Substitute.For<IApiRequestAccessor>();
            var operatorId         = Guid.NewGuid();
            var correlationId      = Guid.NewGuid();
            apiRequestAccessor.GetAuthorizedUserId()
                              .Returns(operatorId);
            apiRequestAccessor.GetCorrelationId()
                              .Returns(correlationId);
            apiRequestAccessor.GetOperationContext()
                              .Returns(new OperationContext(operatorId, correlationId));
            return apiRequestAccessor;
        });
        ServiceProvider = services.BuildServiceProvider().CreateScope().ServiceProvider;

        Mapper    = ServiceProvider.GetRequiredService<IMapper>();
        Mediator  = ServiceProvider.GetRequiredService<IMediator>();
        DbContext = ServiceProvider.GetRequiredService<EventAnalyticDbContext>();
        TestData  = new TestData(DbContext);
        var apiRequestAccessor = ServiceProvider.GetRequiredService<IApiRequestAccessor>();
        OperatorContext = apiRequestAccessor.GetOperationContext();

        TestData.CreateUser(OperatorContext.UserId);
    }
}