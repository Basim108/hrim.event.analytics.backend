using System.Diagnostics.CodeAnalysis;
using System.Net;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.Analysis;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;
using Hrim.Event.Analytics.Api.Tests.Infrastructure.TestingHost;
using Hrim.Event.Analytics.Api.V1.Models;
using Hrimsoft.StringCases;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Hrim.Event.Analytics.Api.Tests.ValidationTests.EventType;

[ExcludeFromCodeCoverage]
public class EventTypeControllerValidationTests: BaseEntityControllerTests
{
    public EventTypeControllerValidationTests(EventAnalyticsWebAppFactory<Program> factory) { Client = factory.GetClient(baseUrl: "v1/event-type/"); }

    /// <summary> Correct create event type request  </summary>
    protected override CreateEventTypeRequest GetCreateRequest() {
        return new CreateEventTypeRequest {
            Name        = "Headache",
            Color       = "#ff0000",
            Description = "times when I had a headache",
            IsPublic    = true
        };
    }

    /// <summary> Correct update event type request  </summary>
    protected override UpdateEventTypeRequest GetUpdateRequest() {
        return new UpdateEventTypeRequest {
            Id              = Guid.NewGuid(),
            ConcurrentToken = 1,
            Name            = "Headache",
            Color           = "#ff0000",
            Description     = "times when I had a headache",
            IsPublic        = true
        };
    }

    [Fact]
    public async Task Create_Given_Empty_Name_Returns_BadRequest() {
        var createRequest = GetCreateRequest();
        createRequest.Name = "";
        var response = await Client!.PostAsync(requestUri: "", TestUtils.PrepareJson(instance: createRequest));
        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(createRequest.Name).ToSnakeCase())
                      .Should()
                      .BeTrue();
    }

    [Fact]
    public async Task Create_Given_NonExisting_CreatById_Returns_BadRequest() {
        var entityToCreate = GetCreateRequest();
        entityToCreate.CreatedById = Guid.NewGuid();

        var response = await Client!.PostAsync(requestUri: "", TestUtils.PrepareJson(entityToCreate));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(IHasOwner.CreatedById).ToSnakeCase())
                      .Should()
                      .BeTrue();
    }

    [Fact]
    public async Task Create_Given_Color_In_Wrong_Format_Returns_BadRequest() {
        var createRequest = GetCreateRequest();
        createRequest.Color = "123456789";
        var response = await Client!.PostAsync(requestUri: "", TestUtils.PrepareJson(instance: createRequest));
        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(createRequest.Color).ToSnakeCase())
                      .Should()
                      .BeTrue();
    }

    [Fact]
    public async Task Create_Given_Color_In_Correct_Long_Hex_Format() {
        var createRequest = GetCreateRequest();
        createRequest.Name = "";
        var response = await Client!.PostAsync(requestUri: "", TestUtils.PrepareJson(instance: createRequest));
        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(createRequest.Color).ToSnakeCase())
                      .Should()
                      .BeFalse();
    }

    [Fact]
    public async Task Create_Given_Color_In_Correct_Short_Hex_Format() {
        var createRequest = GetCreateRequest();
        createRequest.Color = "#f00";
        createRequest.Name  = "";
        var response = await Client!.PostAsync(requestUri: "", TestUtils.PrepareJson(instance: createRequest));
        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(createRequest.Color).ToSnakeCase())
                      .Should()
                      .BeFalse();
    }

    [Fact]
    public async Task Update_Given_Color_In_Correct_Long_Hex_Format() {
        var updateRequest = GetUpdateRequest();
        updateRequest.Name = "";
        var response = await Client!.PutAsync(requestUri: "", TestUtils.PrepareJson(instance: updateRequest));
        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(updateRequest.Color).ToSnakeCase())
                      .Should()
                      .BeFalse();
    }

    [Fact]
    public async Task Update_Given_Color_In_Correct_Short_Hex_Format() {
        var updateRequest = GetUpdateRequest();
        updateRequest.Color = "#f00";
        updateRequest.Name  = "";
        var response = await Client!.PutAsync(requestUri: "", TestUtils.PrepareJson(instance: updateRequest));
        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(updateRequest.Color).ToSnakeCase())
                      .Should()
                      .BeFalse();
    }

    [Fact]
    public async Task Update_Given_Empty_Name_Returns_BadRequest() {
        var updateRequest = GetUpdateRequest();
        updateRequest.Name = "";
        var response = await Client!.PutAsync(requestUri: "", TestUtils.PrepareJson(instance: updateRequest));
        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(updateRequest.Name).ToSnakeCase())
                      .Should()
                      .BeTrue();
    }

    [Fact]
    public async Task Update_Given_Color_In_Wrong_Format_Returns_BadRequest() {
        var updateRequest = GetUpdateRequest();
        updateRequest.Color = "123456789";
        var response = await Client!.PostAsync(requestUri: "", TestUtils.PrepareJson(instance: updateRequest));
        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(updateRequest.Color).ToSnakeCase())
                      .Should()
                      .BeTrue();
    }

    [Fact]
    public async Task Update_Given_NonExisting_CreatById_Returns_BadRequest() {
        GetUpdateRequest().CreatedById = Guid.NewGuid();

        var response = await Client!.PutAsync(requestUri: "", TestUtils.PrepareJson(GetUpdateRequest()));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(IHasOwner.CreatedById).ToSnakeCase())
                      .Should()
                      .BeTrue();
    }
    
    [Fact]
    public async Task Create_Given_Unknown_Analysis_Code_Returns_BadRequest() {
        var entityToCreate = GetCreateRequest();
        entityToCreate.AnalysisSettings = new List<AnalysisByEventType> {
            new () { AnalysisCode = "SomeCode" }
        };

        var response = await Client!.PostAsync(requestUri: "", TestUtils.PrepareJson(entityToCreate));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey("analysis_settings[0].analysis_code")
                      .Should().BeTrue();
        problemDetails.Errors["analysis_settings[0].analysis_code"]
                      .Should().StartWith("Unsupported analysis code");
    }
    
    [Fact]
    public async Task Update_Given_Unknown_Analysis_Code_Returns_BadRequest() {
        var entityToCreate = GetUpdateRequest();
        entityToCreate.AnalysisSettings = new List<AnalysisByEventType> {
            new () { AnalysisCode = "SomeCode" }
        };

        var response = await Client!.PutAsync(requestUri: "", TestUtils.PrepareJson(entityToCreate));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey("analysis_settings[0].analysis_code")
                      .Should().BeTrue();
        problemDetails.Errors["analysis_settings[0].analysis_code"]
                      .Should().StartWith("Unsupported analysis code");
    }
    
    [Fact]
    public async Task Create_Given_Count_Analysis_And_NotEmpty_Settings_Returns_BadRequest() {
        var entityToCreate = GetCreateRequest();
        entityToCreate.AnalysisSettings = new List<AnalysisByEventType> {
            new () {
                AnalysisCode = FeatureCodes.COUNT_ANALYSIS,
                Settings = new Dictionary<string, string>() { {"prop", "value"} }
            }
        };

        var response = await Client!.PostAsync(requestUri: "", TestUtils.PrepareJson(entityToCreate));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey("analysis_settings[0].settings")
                      .Should().BeTrue();
        problemDetails.Errors["analysis_settings[0].settings"]
                      .Should().StartWith("Analysis should have no settings");
    }
    
    [Fact]
    public async Task Update_Given_Count_Analysis_And_NotEmpty_Settings_Returns_BadRequest() {
        var entityToCreate = GetUpdateRequest();
        entityToCreate.AnalysisSettings = new List<AnalysisByEventType> {
            new () {
                AnalysisCode = FeatureCodes.COUNT_ANALYSIS,
                Settings     = new Dictionary<string, string>() { {"prop", "value"} }
            }
        };

        var response = await Client!.PutAsync(requestUri: "", TestUtils.PrepareJson(entityToCreate));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey("analysis_settings[0].settings")
                      .Should().BeTrue();
        problemDetails.Errors["analysis_settings[0].settings"]
                      .Should().StartWith("Analysis should have no settings");
    }
    
    [Fact]
    public async Task Create_Given_Gap_Analysis_And_Unknown_Settings_Returns_BadRequest() {
        var entityToCreate = GetCreateRequest();
        entityToCreate.AnalysisSettings = new List<AnalysisByEventType> {
            new () {
                AnalysisCode = FeatureCodes.GAP_ANALYSIS,
                Settings = new Dictionary<string, string>() { {"prop", "value"} }
            }
        };

        var response = await Client!.PostAsync(requestUri: "", TestUtils.PrepareJson(entityToCreate));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey("analysis_settings[0].settings[0]")
                      .Should().BeTrue();
        problemDetails.Errors["analysis_settings[0].settings[0]"]
                      .Should().StartWith("Unsupported gap analysis setting");
    }
    
    [Fact]
    public async Task Update_Given_Gap_Analysis_And_Unknown_Settings_Returns_BadRequest() {
        var entityToCreate = GetUpdateRequest();
        entityToCreate.AnalysisSettings = new List<AnalysisByEventType> {
            new () {
                AnalysisCode = FeatureCodes.GAP_ANALYSIS,
                Settings     = new Dictionary<string, string>() { {"prop", "value"} }
            }
        };

        var response = await Client!.PutAsync(requestUri: "", TestUtils.PrepareJson(entityToCreate));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey("analysis_settings[0].settings[0]")
                      .Should().BeTrue();
        problemDetails.Errors["analysis_settings[0].settings[0]"]
                      .Should().StartWith("Unsupported gap analysis setting");
    }
    
    [Fact]
    public async Task Create_Given_Gap_Analysis_And_Additional_Unknown_Settings_Returns_BadRequest() {
        var entityToCreate = GetCreateRequest();
        entityToCreate.AnalysisSettings = new List<AnalysisByEventType> {
            new () {
                AnalysisCode = FeatureCodes.GAP_ANALYSIS,
                Settings     = new Dictionary<string, string>() {
                    {AnalysisSettingNames.Gap.MINIMAL_GAP_LENGTH, "1d"},
                    {"prop", "value"}
                }
            }
        };

        var response = await Client!.PostAsync(requestUri: "", TestUtils.PrepareJson(entityToCreate));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors.Count.Should().Be(1);
        problemDetails.Errors
                      .ContainsKey("analysis_settings[0].settings[1]")
                      .Should().BeTrue();
        problemDetails.Errors["analysis_settings[0].settings[1]"]
                      .Should().StartWith("Unsupported gap analysis setting");
    }
    
    [Fact]
    public async Task Update_Given_Gap_Analysis_And_Additional_Unknown_Settings_Returns_BadRequest() {
        var entityToCreate = GetUpdateRequest();
        entityToCreate.AnalysisSettings = new List<AnalysisByEventType> {
            new () {
                AnalysisCode = FeatureCodes.GAP_ANALYSIS,
                Settings = new Dictionary<string, string>() {
                    {AnalysisSettingNames.Gap.MINIMAL_GAP_LENGTH, "1d"},
                    {"prop", "value"}
                }
            }
        };

        var response = await Client!.PutAsync(requestUri: "", TestUtils.PrepareJson(entityToCreate));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors.Count.Should().Be(1);
        problemDetails.Errors
                      .ContainsKey("analysis_settings[0].settings[1]")
                      .Should().BeTrue();
        problemDetails.Errors["analysis_settings[0].settings[1]"]
                      .Should().StartWith("Unsupported gap analysis setting");
    }
    
    [Fact]
    public async Task Create_Given_Gap_Analysis_And_MinimalGapLength_More_Than_128_Returns_BadRequest() {
        var entityToCreate = GetCreateRequest();
        entityToCreate.AnalysisSettings = new List<AnalysisByEventType> {
            new () {
                AnalysisCode = FeatureCodes.GAP_ANALYSIS,
                Settings = new Dictionary<string, string>() {
                    {AnalysisSettingNames.Gap.MINIMAL_GAP_LENGTH, TestUtils.GenerateString(129)}
                }
            }
        };

        var response = await Client!.PostAsync(requestUri: "", TestUtils.PrepareJson(entityToCreate));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors.Count.Should().Be(1);
        problemDetails.Errors
                      .ContainsKey("analysis_settings[0].settings[0]")
                      .Should().BeTrue();
        problemDetails.Errors["analysis_settings[0].settings[0]"]
                      .Should().StartWith("is too long. must be less then 128");
    }
    
    [Fact]
    public async Task Update_Given_Gap_Analysis_And_MinimalGapLength_More_Than_128_Returns_BadRequest() {
        var entityToCreate = GetUpdateRequest();
        entityToCreate.AnalysisSettings = new List<AnalysisByEventType> {
            new () {
                AnalysisCode = FeatureCodes.GAP_ANALYSIS,
                Settings = new Dictionary<string, string>() {
                    {AnalysisSettingNames.Gap.MINIMAL_GAP_LENGTH, TestUtils.GenerateString(129)}
                }
            }
        };

        var response = await Client!.PutAsync(requestUri: "", TestUtils.PrepareJson(entityToCreate));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors.Count.Should().Be(1);
        problemDetails.Errors
                      .ContainsKey("analysis_settings[0].settings[0]")
                      .Should().BeTrue();
        problemDetails.Errors["analysis_settings[0].settings[0]"]
                      .Should().StartWith("is too long. must be less then 128");
    }
}