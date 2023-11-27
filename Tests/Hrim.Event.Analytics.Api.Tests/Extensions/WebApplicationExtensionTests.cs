using FluentAssertions;
using Hrim.Event.Analytics.Api.Extensions;
using Hrimsoft.Core.Exceptions;
using Microsoft.Extensions.Configuration;

namespace Hrim.Event.Analytics.Api.Tests.Extensions;

public class WebApplicationExtensionTests
{
    [Fact]
    public void UseEventAnalyticsCors_Should_Throw_Exception_When_No_ALLOWED_ORIGINS() {
        var appConfig = new ConfigurationBuilder()
                       // .AddInMemoryCollection(new Dictionary<string, string>())
                       .Build();
        var ex = Assert.Throws<ConfigurationException>(() => WebApplicationExtensions.UseEventAnalyticsCors(app: null, appConfig: appConfig));
        ex.Section.Should().Be(expected: "root");
        ex.Key.Should().Be(expected: "ALLOWED_ORIGINS");
    }
}