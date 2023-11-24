using System.Diagnostics.CodeAnalysis;
using System.Net;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Api.Services;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;
using Hrim.Event.Analytics.Api.Tests.Infrastructure.TestingHost;
using Hrim.Event.Analytics.Api.V1.Models;
using Hrimsoft.StringCases;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ApiProgram = Hrim.Event.Analytics.Api.Program;

namespace Hrim.Event.Analytics.Api.Tests.ValidationTests;

/// <summary>
///     Tests entity properties and common endpoints validation
/// </summary>
[ExcludeFromCodeCoverage]
[SuppressMessage(category: "Usage",
                 checkId:
                 "xUnit1033:Test classes decorated with \'Xunit.IClassFixture<TFixture>\' or \'Xunit.ICollectionFixture<TFixture>\' should add a constructor argument of type TFixture")]
public abstract class BaseEntityControllerTests: IClassFixture<EventAnalyticsWebAppFactory<ApiProgram>>
{
    protected HttpClient? Client { get; init; }

    protected JsonSerializerSettings JsonSettings { get; set; } = JsonSettingsFactory.Get();

    protected abstract HrimEntity<long> GetCreateRequest();

    protected abstract HrimEntity<long> GetUpdateRequest();

    [Theory]
    [InlineData("0")]
    [InlineData("-2")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("352246af-9681")]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552-352246af-9681-4aae-9c2c-6faddcb2e552")]
    public async Task GetById_Given_Wrong_Id_Returns_BadRequest(string url) {
        var response = await Client!.GetAsync(requestUri: url);
        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(ByIdRequest<long>.Id).ToSnakeCase())
                      .Should()
                      .BeTrue();
    }

    [Fact]
    public async Task Create_Given_Id_Returns_BadRequest() {
        var createRequest = GetCreateRequest();
        createRequest.Id = new Random().NextInt64();
        var response = await Client!.PostAsync(requestUri: "", TestUtils.PrepareJson(instance: createRequest));
        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(createRequest.Id).ToSnakeCase())
                      .Should()
                      .BeTrue();
    }

    [Fact]
    public async Task Create_Given_Positive_ConcurrentToken_Returns_BadRequest() {
        var createRequest = GetCreateRequest();
        createRequest.ConcurrentToken = 1;
        var response = await Client!.PostAsync(requestUri: "", TestUtils.PrepareJson(instance: createRequest));
        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(createRequest.ConcurrentToken).ToSnakeCase())
                      .Should()
                      .BeTrue();
    }

    [Fact]
    public async Task Update_Given_Empty_Id_Returns_BadRequest() {
        var updateRequest = GetUpdateRequest();
        updateRequest.Id = default;
        var response = await Client!.PutAsync(requestUri: "", TestUtils.PrepareJson(instance: updateRequest));
        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(updateRequest.Id).ToSnakeCase())
                      .Should()
                      .BeTrue();
    }

    [Fact]
    public async Task Update_Given_0_ConcurrentToken_Returns_BadRequest() {
        var updateRequest = GetCreateRequest();
        updateRequest.ConcurrentToken = 0;
        var response = await Client!.PutAsync(requestUri: "", TestUtils.PrepareJson(instance: updateRequest));
        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(updateRequest.ConcurrentToken).ToSnakeCase())
                      .Should()
                      .BeTrue();
    }
}