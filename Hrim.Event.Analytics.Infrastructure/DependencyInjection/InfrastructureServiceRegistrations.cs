using Hrim.Event.Analytics.Infrastructure.Mediator;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Hrim.Event.Analytics.Infrastructure.DependencyInjection;

public static class InfrastructureServiceRegistrations
{
    public static void AddEventAnalyticsInfrastructure(this IServiceCollection services) { services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PrePostLogWrapper<,>)); }
}