using System.Diagnostics.CodeAnalysis;
using System.Net;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;
using Hrim.Event.Analytics.Api.Tests.Infrastructure.TestingHost;
using Hrim.Event.Analytics.Api.V1.Models;
using Hrimsoft.StringCases;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Hrim.Event.Analytics.Api.Tests.ValidationTests.EventType;

[ExcludeFromCodeCoverage]
public class EventTypeControllerValidationTests : BaseEntityControllerTests
{
    public EventTypeControllerValidationTests(WebAppFactory<Program> factory)
    {
        Client = factory.GetClient("v1/event-type/");
    }

    /// <summary> Correct create event type request  </summary>
    protected override CreateEventTypeRequest GetCreateRequest()
    {
        return new()
        {
            Name = "Headache",
            Color = "#ff0000",
            Description = "times when I had a headache",
            IsPublic = true
        };
    }

    /// <summary> Correct update event type request  </summary>
    protected override UpdateEventTypeRequest GetUpdateRequest()
    {
        return new()
        {
            Id = Guid.NewGuid(),
            ConcurrentToken = 1,
            Name = "Headache",
            Color = "#ff0000",
            Description = "times when I had a headache",
            IsPublic = true
        };
    }

    [Fact]
    public async Task Create_Given_Empty_Name_Returns_BadRequest()
    {
        var createRequest = GetCreateRequest();
        createRequest.Name = "";
        var response = await Client!.PostAsync("", TestUtils.PrepareJson(createRequest));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
            .ContainsKey(nameof(createRequest.Name).ToSnakeCase())
            .Should().BeTrue();
    }

    [Fact]
    public async Task Create_Given_NonExisting_CreatById_Returns_BadRequest()
    {
        GetCreateRequest().CreatedById = Guid.NewGuid();

        var response = await Client!.PostAsync("", TestUtils.PrepareJson(GetCreateRequest()));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
            .ContainsKey(nameof(IHasOwner.CreatedById).ToSnakeCase())
            .Should().BeTrue();
    }

    [Fact]
    public async Task Create_Given_Color_In_Wrong_Format_Returns_BadRequest()
    {
        var createRequest = GetCreateRequest();
        createRequest.Color = "123456789";
        var response = await Client!.PostAsync("", TestUtils.PrepareJson(createRequest));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
            .ContainsKey(nameof(createRequest.Color).ToSnakeCase())
            .Should().BeTrue();
    }

    [Fact]
    public async Task Create_Given_Color_In_Correct_Long_Hex_Format()
    {
        var createRequest = GetCreateRequest();
        createRequest.Name = "";
        var response = await Client!.PostAsync("", TestUtils.PrepareJson(createRequest));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
            .ContainsKey(nameof(createRequest.Color).ToSnakeCase())
            .Should().BeFalse();
    }

    [Fact]
    public async Task Create_Given_Color_In_Correct_Short_Hex_Format()
    {
        var createRequest = GetCreateRequest();
        createRequest.Color = "#f00";
        createRequest.Name = "";
        var response = await Client!.PostAsync("", TestUtils.PrepareJson(createRequest));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
            .ContainsKey(nameof(createRequest.Color).ToSnakeCase())
            .Should().BeFalse();
    }

    [Fact]
    public async Task Update_Given_Color_In_Correct_Long_Hex_Format()
    {
        var updateRequest = GetUpdateRequest();
        updateRequest.Name = "";
        var response = await Client!.PutAsync("", TestUtils.PrepareJson(updateRequest));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
            .ContainsKey(nameof(updateRequest.Color).ToSnakeCase())
            .Should().BeFalse();
    }

    [Fact]
    public async Task Update_Given_Color_In_Correct_Short_Hex_Format()
    {
        var updateRequest = GetUpdateRequest();
        updateRequest.Color = "#f00";
        updateRequest.Name = "";
        var response = await Client!.PutAsync("", TestUtils.PrepareJson(updateRequest));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
            .ContainsKey(nameof(updateRequest.Color).ToSnakeCase())
            .Should().BeFalse();
    }

    [Fact]
    public async Task Update_Given_Empty_Name_Returns_BadRequest()
    {
        var updateRequest = GetUpdateRequest();
        updateRequest.Name = "";
        var response = await Client!.PutAsync("", TestUtils.PrepareJson(updateRequest));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
            .ContainsKey(nameof(updateRequest.Name).ToSnakeCase())
            .Should().BeTrue();
    }

    [Fact]
    public async Task Update_Given_Color_In_Wrong_Format_Returns_BadRequest()
    {
        var updateRequest = GetUpdateRequest();
        updateRequest.Color = "123456789";
        var response = await Client!.PostAsync("", TestUtils.PrepareJson(updateRequest));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
            .ContainsKey(nameof(updateRequest.Color).ToSnakeCase())
            .Should().BeTrue();
    }

    [Fact]
    public async Task Update_Given_NonExisting_CreatById_Returns_BadRequest()
    {
        GetUpdateRequest().CreatedById = Guid.NewGuid();

        var response = await Client!.PutAsync("", TestUtils.PrepareJson(GetUpdateRequest()));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
            .ContainsKey(nameof(IHasOwner.CreatedById).ToSnakeCase())
            .Should().BeTrue();
    }
}