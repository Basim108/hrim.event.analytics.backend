using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs.Features;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;
using Hrim.Event.Analytics.EfCore.Cqrs.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Features;

[ExcludeFromCodeCoverage]
public class SetupFeaturesTests
{
    private readonly TestData      _testData = new(DbUtils.GetDbContext());
    private readonly SetupFeatures _command  = new();

    [Fact]
    public async Task Given_Gap_When_No_Diff_With_Env_Should_Not_Change_DB() {
        var appConfig = new ConfigurationBuilder()
                       .AddInMemoryCollection(new Dictionary<string, string>() {
                            {
                                "FEAT_GAP", "On"
                            }
                        }!)
                       .Build();

        var gap = _testData.Features.EnsureExistence("FEAT_GAP", FeatureCodes.GAP_ANALYSIS, true);
        gap.UpdatedAt.Should().BeNull();

        var handler = new SetupFeaturesHandler(_testData.DbContext, NullLogger<SetupFeaturesHandler>.Instance, appConfig);
        await handler.Handle(_command, CancellationToken.None);

        var actualGap = _testData.DbContext.HrimFeatures.FirstOrDefault(x => x.Code == FeatureCodes.GAP_ANALYSIS);
        actualGap.Should().NotBeNull();
        actualGap!.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public async Task Given_Gap_When_There_Is_Diff_With_Env_Should_Update_DB() {
        var appConfig = new ConfigurationBuilder()
                       .AddInMemoryCollection(new Dictionary<string, string>() {
                            {
                                "FEAT_GAP", "Off"
                            }
                        }!)
                       .Build();

        var gap = _testData.Features.EnsureExistence("FEAT_GAP", FeatureCodes.GAP_ANALYSIS, true);
        gap.UpdatedAt.Should().BeNull();

        var handler = new SetupFeaturesHandler(_testData.DbContext, NullLogger<SetupFeaturesHandler>.Instance, appConfig);
        await handler.Handle(_command, CancellationToken.None);

        var actualGap = _testData.DbContext.HrimFeatures.FirstOrDefault(x => x.Code == FeatureCodes.GAP_ANALYSIS);
        actualGap.Should().NotBeNull();
        actualGap!.UpdatedAt.Should().NotBeNull();
        actualGap.IsOn.Should().BeFalse();
    }

    [Fact]
    public async Task Given_Gap_On_When_Env_Off_Should_Set_DB_To_Off() {
        var appConfig = new ConfigurationBuilder()
                       .AddInMemoryCollection(new Dictionary<string, string>() {
                            {
                                "FEAT_GAP", "Off"
                            }
                        }!)
                       .Build();

        var gap = _testData.Features.EnsureExistence("FEAT_GAP", FeatureCodes.GAP_ANALYSIS, true);
        gap.UpdatedAt.Should().BeNull();

        var handler = new SetupFeaturesHandler(_testData.DbContext, NullLogger<SetupFeaturesHandler>.Instance, appConfig);
        await handler.Handle(_command, CancellationToken.None);

        var actualGap = _testData.DbContext.HrimFeatures.FirstOrDefault(x => x.Code == FeatureCodes.GAP_ANALYSIS);
        actualGap.Should().NotBeNull();
        actualGap!.UpdatedAt.Should().NotBeNull();
        actualGap.IsOn.Should().BeFalse();
    }

    [Fact]
    public async Task Given_Gap_Off_When_Env_On_Should_Set_DB_To_On() {
        var appConfig = new ConfigurationBuilder()
                       .AddInMemoryCollection(new Dictionary<string, string>() {
                            {
                                "FEAT_GAP", "On"
                            }
                        }!)
                       .Build();

        var gap = _testData.Features.EnsureExistence("FEAT_GAP", FeatureCodes.GAP_ANALYSIS, false);
        gap.UpdatedAt.Should().BeNull();

        var handler = new SetupFeaturesHandler(_testData.DbContext, NullLogger<SetupFeaturesHandler>.Instance, appConfig);
        await handler.Handle(_command, CancellationToken.None);

        var actualGap = _testData.DbContext.HrimFeatures.FirstOrDefault(x => x.Code == FeatureCodes.GAP_ANALYSIS);
        actualGap.Should().NotBeNull();
        actualGap!.UpdatedAt.Should().NotBeNull();
        actualGap.IsOn.Should().BeTrue();
    }

    [Fact]
    public async Task Given_Changes_DB_Should_Increment_ConcurrentToken() {
        var appConfig = new ConfigurationBuilder()
                       .AddInMemoryCollection(new Dictionary<string, string>() {
                            {
                                "FEAT_GAP", "On"
                            }
                        }!)
                       .Build();

        var gap = _testData.Features.EnsureExistence("FEAT_GAP", FeatureCodes.GAP_ANALYSIS, false);
        gap.UpdatedAt.Should().BeNull();

        var handler = new SetupFeaturesHandler(_testData.DbContext, NullLogger<SetupFeaturesHandler>.Instance, appConfig);
        await handler.Handle(_command, CancellationToken.None);

        var actualGap = _testData.DbContext.HrimFeatures.FirstOrDefault(x => x.Code == FeatureCodes.GAP_ANALYSIS);
        actualGap.Should().NotBeNull();
        actualGap!.ConcurrentToken.Should().Be(1);
    }
}