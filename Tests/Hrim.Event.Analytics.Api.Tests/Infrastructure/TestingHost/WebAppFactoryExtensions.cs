using Microsoft.AspNetCore.Mvc.Testing;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure.TestingHost;

public static class WebAppFactoryExtensions
{
    public static HttpClient GetClient(this EventAnalyticsWebAppFactory<Program> factory, string baseUrl) {
        return factory.CreateClient(new WebApplicationFactoryClientOptions {
            BaseAddress = new Uri($"https://localhost:7151/{baseUrl}")
        });
    }
}