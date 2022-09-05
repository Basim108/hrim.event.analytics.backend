using System.Diagnostics.CodeAnalysis;
using System.Net;
using FluentAssertions;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;
using Hrim.Event.Analytics.Api.Tests.Infrastructure.TestingHost;
using Hrim.Event.Analytics.Api.V1.Models;
using Hrimsoft.StringCases;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Hrim.Event.Analytics.Api.Tests.ValidationTests.Entity;

[ExcludeFromCodeCoverage]
public class EntityControllerValidationTests: IClassFixture<WebAppFactory<Program>> {
    private readonly HttpClient _client;

    public EntityControllerValidationTests(WebAppFactory<Program> factory) {
        _client = factory.GetClient("v1/entity/");
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("352246af-9681")]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552-352246af-9681-4aae-9c2c-6faddcb2e552")]
    public async Task Restore_Given_Wrong_Id_Returns_BadRequest(string url) {
        var response = await _client.PatchAsync(url, new StringContent(""));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(ByIdRequest.Id).ToSnakeCase())
                      .Should().BeTrue();
    }

    [Theory]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552")]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552?entity_type=")]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552?entity_type=Hello")]
    public async Task Restore_Given_Wrong_EntityType_Returns_BadRequest(string url) {
        var response = await _client.PatchAsync(url, new StringContent(""));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey("entity_type")
                      .Should().BeTrue();
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("352246af-9681")]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552-352246af-9681-4aae-9c2c-6faddcb2e552")]
    public async Task Delete_Given_Wrong_Id_Returns_BadRequest(string url) {
        var response = await _client.DeleteAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(ByIdRequest.Id).ToSnakeCase())
                      .Should().BeTrue();
    }

    [Theory]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552")]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552?entity_type=")]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552?entity_type=Hello")]
    public async Task Delete_Given_Wrong_EntityType_Returns_BadRequest(string url) {
        var response = await _client.DeleteAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey("entity_type")
                      .Should().BeTrue();
    }
}