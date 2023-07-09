using System.Diagnostics.CodeAnalysis;
using System.Net;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.Analysis;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;
using Hrim.Event.Analytics.Api.Tests.Infrastructure.TestingHost;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Hrim.Event.Analytics.Api.Tests.ValidationTests.Analysis;

[ExcludeFromCodeCoverage]
public class AnalysisControllerValidationTests: IClassFixture<EventAnalyticsWebAppFactory<Program>>
{
    private readonly HttpClient? _client;
    
    public AnalysisControllerValidationTests(EventAnalyticsWebAppFactory<Program> factory) {
        _client = factory.GetClient(baseUrl: "v1/analysis/event-type/");
    }
    
    [Fact]
    public async Task Update_Given_Unknown_Analysis_Code_Returns_BadRequest() {
        var list = new List<AnalysisByEventType> {
            new () { AnalysisCode = "SomeCode" }
        };

        var response = await _client!.PostAsync(requestUri: Guid.NewGuid().ToString(), TestUtils.PrepareJson(list));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey("analysis[0].analysis_code")
                      .Should().BeTrue();
        problemDetails.Errors["analysis[0].analysis_code"]
                      .Should().StartWith("Unsupported analysis code");
    }
    
    [Fact]
    public async Task Update_Given_Count_Analysis_And_NotEmpty_Settings_Returns_BadRequest() {
        var list = new List<AnalysisByEventType> {
            new () {
                AnalysisCode = FeatureCodes.COUNT_ANALYSIS,
                Settings = new Dictionary<string, string>() { {"prop", "value"} }
            }
        };

        var response = await _client!.PostAsync(requestUri: Guid.NewGuid().ToString(), TestUtils.PrepareJson(list));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey("analysis[0].settings")
                      .Should().BeTrue();
        problemDetails.Errors["analysis[0].settings"]
                      .Should().StartWith("Analysis should have no settings");
    }
    
    [Fact]
    public async Task Update_Given_Gap_Analysis_And_Unknown_Settings_Returns_BadRequest() {
        var list = new List<AnalysisByEventType> {
            new () {
                AnalysisCode = FeatureCodes.GAP_ANALYSIS,
                Settings = new Dictionary<string, string>() { {"prop", "value"} }
            }
        };

        var response = await _client!.PostAsync(requestUri: Guid.NewGuid().ToString(), TestUtils.PrepareJson(list));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey("analysis[0].settings[0]")
                      .Should().BeTrue();
        problemDetails.Errors["analysis[0].settings[0]"]
                      .Should().StartWith("Unsupported gap analysis setting");
    }

    [Fact]
    public async Task Update_Given_Gap_Analysis_And_Additional_Unknown_Settings_Returns_BadRequest() {
        var list = new List<AnalysisByEventType> {
            new () {
                AnalysisCode = FeatureCodes.GAP_ANALYSIS,
                Settings     = new Dictionary<string, string>() {
                    {AnalysisSettingNames.Gap.MINIMAL_GAP_LENGTH, "1d"},
                    {"prop", "value"}
                }
            }
        };

        var response = await _client!.PostAsync(requestUri: Guid.NewGuid().ToString(), TestUtils.PrepareJson(list));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors.Count.Should().Be(1);
        problemDetails.Errors
                      .ContainsKey("analysis[0].settings[1]")
                      .Should().BeTrue();
        problemDetails.Errors["analysis[0].settings[1]"]
                      .Should().StartWith("Unsupported gap analysis setting");
    }
    
    [Fact]
    public async Task Update_Given_Gap_Analysis_And_MinimalGapLength_More_Than_128_Returns_BadRequest() {
        var list = new List<AnalysisByEventType> {
            new () {
                AnalysisCode = FeatureCodes.GAP_ANALYSIS,
                Settings = new Dictionary<string, string>() {
                    {AnalysisSettingNames.Gap.MINIMAL_GAP_LENGTH, TestUtils.GenerateString(129)}
                }
            }
        };

        var response = await _client!.PostAsync(requestUri: Guid.NewGuid().ToString(), TestUtils.PrepareJson(list));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors.Count.Should().Be(1);
        problemDetails.Errors
                      .ContainsKey("analysis[0].settings[0]")
                      .Should().BeTrue();
        problemDetails.Errors["analysis[0].settings[0]"]
                      .Should().StartWith("is too long. must be less then 128");
    }
}