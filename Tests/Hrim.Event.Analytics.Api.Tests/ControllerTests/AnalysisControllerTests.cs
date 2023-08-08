using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.Api.Tests.CqrsTests;
using Hrim.Event.Analytics.Api.V1.Controllers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Hrim.Event.Analytics.Api.Tests.ControllerTests;

[ExcludeFromCodeCoverage]
public class AnalysisControllerTests: BaseCqrsTests
{
    private readonly AnalysisController _controller;

    public AnalysisControllerTests() {
        _controller = new AnalysisController(ServiceProvider.GetRequiredService<IApiRequestAccessor>(),
                                             ServiceProvider.GetRequiredService<IAnalysisSettingsFactory>(),
                                             ServiceProvider.GetRequiredService<IMediator>());
    }

    [Fact]
    public async Task Should_Get_All_Available_Analysis() {
        TestData.Features.EnsureExistence("FEAT_GAP",   FeatureCodes.GAP_ANALYSIS,   false);
        TestData.Features.EnsureExistence("FEAT_COUNT", FeatureCodes.COUNT_ANALYSIS, true, "explanation");

        var resultList = await _controller.GetAllAsync(CancellationToken.None);

        resultList.Should().NotBeNullOrEmpty();
        resultList.Count.Should().Be(1);
        resultList[0].Code.Should().Be(FeatureCodes.COUNT_ANALYSIS);
        resultList[0].Description.Should().Be("explanation");
    }
}