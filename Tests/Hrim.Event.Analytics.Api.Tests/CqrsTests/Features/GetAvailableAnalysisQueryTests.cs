using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs.Features;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Features;

[ExcludeFromCodeCoverage]
public class GetAvailableAnalysisQueryTests: BaseCqrsTests
{
    private readonly GetAvailableAnalysisQuery _query = new();
    [Fact]
    public async Task Given_2_Analysis_When_Both_On_Should_Return_Both() {
        TestData.Features.EnsureExistence( "FEAT_GAP", FeatureCodes.GAP_ANALYSIS, true);
        TestData.Features.EnsureExistence( "FEAT_COUNT", FeatureCodes.COUNT_ANALYSIS, true);

        var resultList = (await Mediator.Send(_query, CancellationToken.None)).ToList();

        resultList.Should().NotBeNullOrEmpty();
        resultList.Count.Should().Be(2);
        resultList.Any(x => x.Code == FeatureCodes.COUNT_ANALYSIS).Should().BeTrue();
        resultList.Any(x => x.Code == FeatureCodes.GAP_ANALYSIS).Should().BeTrue();
    }
    
    [Fact]
    public async Task Given_2_Analysis_When_Both_Off_Should_Return_EmptyList() {
        TestData.Features.EnsureExistence( "FEAT_GAP",   FeatureCodes.GAP_ANALYSIS,   false);
        TestData.Features.EnsureExistence( "FEAT_COUNT", FeatureCodes.COUNT_ANALYSIS, false);

        var resultList = (await Mediator.Send(_query, CancellationToken.None)).ToList();

        resultList.Should().NotBeNull();
        resultList.Should().BeEmpty();
    }
    
    [Fact]
    public async Task Given_2_Analysis_When_One_On_Should_Return_It() {
        TestData.Features.EnsureExistence( "FEAT_GAP",   FeatureCodes.GAP_ANALYSIS,   false);
        TestData.Features.EnsureExistence( "FEAT_COUNT", FeatureCodes.COUNT_ANALYSIS, true);

        var resultList = (await Mediator.Send(_query, CancellationToken.None)).ToList();

        resultList.Should().NotBeNullOrEmpty();
        resultList.Count.Should().Be(1);
        resultList.Any(x => x.Code == FeatureCodes.COUNT_ANALYSIS).Should().BeTrue();
    }
}