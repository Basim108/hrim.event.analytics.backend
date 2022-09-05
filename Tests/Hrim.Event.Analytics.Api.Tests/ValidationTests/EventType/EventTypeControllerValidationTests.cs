using System.Diagnostics.CodeAnalysis;
using System.Net;
using FluentAssertions;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;
using Hrim.Event.Analytics.Api.Tests.Infrastructure.TestingHost;
using Hrimsoft.StringCases;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Hrim.Event.Analytics.Api.Tests.ValidationTests.EventType;

[ExcludeFromCodeCoverage]
public class EventTypeControllerValidationTests: BaseEntityControllerTests {
    public EventTypeControllerValidationTests(WebAppFactory<Program> factory) {
        Client = factory.GetClient("v1/event-type/");
    }

    [Fact]
    public async Task Create_Given_Empty_Name_Returns_BadRequest() {
        var createRequest = CreateEventTypeRequest;
        createRequest.Name = "";
        var response = await Client!.PostAsync("", TestUtils.PrepareJson(createRequest));
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
        var createRequest = CreateEventTypeRequest;
        createRequest.Color = "123456789";
        var response = await Client!.PostAsync("", TestUtils.PrepareJson(createRequest));
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
        var createRequest = CreateEventTypeRequest;
        createRequest.Name = "";
        var response = await Client!.PostAsync("", TestUtils.PrepareJson(createRequest));
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
        var createRequest = CreateEventTypeRequest;
        createRequest.Color = "#f00";
        createRequest.Name  = "";
        var response = await Client!.PostAsync("", TestUtils.PrepareJson(createRequest));
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
        var updateRequest = UpdateEventTypeRequest;
        updateRequest.Name = "";
        var response = await Client!.PutAsync("", TestUtils.PrepareJson(updateRequest));
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
        var updateRequest = UpdateEventTypeRequest;
        updateRequest.Color = "#f00";
        updateRequest.Name  = "";
        var response = await Client!.PutAsync("", TestUtils.PrepareJson(updateRequest));
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
        var updateRequest = UpdateEventTypeRequest;
        updateRequest.Name = "";
        var response = await Client!.PutAsync("", TestUtils.PrepareJson(updateRequest));
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
        var updateRequest = UpdateEventTypeRequest;
        updateRequest.Color = "123456789";
        var response = await Client!.PostAsync("", TestUtils.PrepareJson(updateRequest));
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