// Copyright © 2021 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

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
                       .AddInMemoryCollection(new Dictionary<string, string>())
                       .Build();
        var ex = Assert.Throws<ConfigurationException>(() => WebApplicationExtensions.UseEventAnalyticsCors(null, appConfig));
        ex.Section.Should().Be("root");
        ex.Key.Should().Be("ALLOWED_ORIGINS");
    }
}