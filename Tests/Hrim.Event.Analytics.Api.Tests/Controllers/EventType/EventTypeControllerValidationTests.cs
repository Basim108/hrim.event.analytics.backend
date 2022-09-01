using System.Net;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;
using Hrim.Event.Analytics.Api.V1.Models;
using Hrimsoft.StringCases;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Hrim.Event.Analytics.Api.Tests.Controllers.EventType;

public class EventTypeControllerValidationTests: BaseEntityControllerTests<UserEventType> {
    private readonly HttpClient _client; 
    public EventTypeControllerValidationTests(WebAppFactory<Program> factory)
        : base(factory) {
        _client = GetClient("/v1/event-type/");
    }

    protected override UserEventType GetCreateRequestEntity() => new CreateEventTypeRequest() {
        Name        = "Headache",
        Color       = "#ff0000",
        Description = "times when I had a headache",
        IsPublic    = true
    };

    protected override UserEventType GetUpdateRequestEntity() => new UpdateEventTypeRequest() {
        Id              = Guid.NewGuid(),
        ConcurrentToken = 1,
        Name            = "Headache",
        Color           = "#ff0000",
        Description     = "times when I had a headache",
        IsPublic        = true
    };

    [Fact]
    public async Task Create_Given_Empty_Name_Returns_BadRequest() {
        var createRequest = GetCreateRequestEntity();
        createRequest.Name = "";
        var response = await _client.PostAsync("", TestUtils.PrepareJson(createRequest));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(createRequest.Name).ToSnakeCase())
                      .Should().BeTrue();
    }

    [Fact]
    public async Task Create_Given_Color_In_Wrong_Format_Returns_BadRequest() {
        var createRequest = GetCreateRequestEntity();
        createRequest.Color = "123456789";
        var response = await _client.PostAsync("", TestUtils.PrepareJson(createRequest));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(createRequest.Color).ToSnakeCase())
                      .Should().BeTrue();
    }

    [Fact]
    public async Task Create_Given_Color_In_Correct_Long_Hex_Format() {
        var createRequest = GetCreateRequestEntity();
        createRequest.Name = "";
        var response = await _client.PostAsync("", TestUtils.PrepareJson(createRequest));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(createRequest.Color).ToSnakeCase())
                      .Should().BeFalse();
    }

    [Fact]
    public async Task Create_Given_Color_In_Correct_Short_Hex_Format() {
        var createRequest = GetCreateRequestEntity();
        createRequest.Color = "#f00";
        createRequest.Name  = "";
        var response = await _client.PostAsync("", TestUtils.PrepareJson(createRequest));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(createRequest.Color).ToSnakeCase())
                      .Should().BeFalse();
    }

    [Fact]
    public async Task Update_Given_Color_In_Correct_Long_Hex_Format() {
        var updateRequest = GetUpdateRequestEntity();
        updateRequest.Name = "";
        var response = await _client.PutAsync("", TestUtils.PrepareJson(updateRequest));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(updateRequest.Color).ToSnakeCase())
                      .Should().BeFalse();
    }

    [Fact]
    public async Task Update_Given_Color_In_Correct_Short_Hex_Format() {
        var updateRequest = GetUpdateRequestEntity();
        updateRequest.Color = "#f00";
        updateRequest.Name  = "";
        var response = await _client.PutAsync("", TestUtils.PrepareJson(updateRequest));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(updateRequest.Color).ToSnakeCase())
                      .Should().BeFalse();
    }

    [Fact]
    public async Task Update_Given_Empty_Name_Returns_BadRequest() {
        var updateRequest = GetUpdateRequestEntity();
        updateRequest.Name = "";
        var response = await _client.PutAsync("", TestUtils.PrepareJson(updateRequest));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(updateRequest.Name).ToSnakeCase())
                      .Should().BeTrue();
    }

    [Fact]
    public async Task Update_Given_Color_In_Wrong_Format_Returns_BadRequest() {
        var updateRequest = GetUpdateRequestEntity();
        updateRequest.Color = "123456789";
        var response = await _client.PostAsync("", TestUtils.PrepareJson(updateRequest));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(updateRequest.Color).ToSnakeCase())
                      .Should().BeTrue();
    }
}