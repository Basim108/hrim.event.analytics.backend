using Hrim.Event.Analytics.FakeHandlers.EventTypes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Hrim.Event.Analytics.FakeHandlers.DependencyInjection; 

public static class ServiceCollectionRegistrations {
    public static void AddFakeCqrsHandlers(this IServiceCollection services) {
        services.AddMediatR(typeof(GetAllPublicUserEventTypesHandler).Assembly);
    }
}