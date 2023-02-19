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

namespace Hrim.Event.Analytics.Api.Tests.ValidationTests;

/// <summary>
///     Tests entity properties and common endpoints validation
/// </summary>
[ExcludeFromCodeCoverage]
[SuppressMessage("Usage",
    "xUnit1033:Test classes decorated with \'Xunit.IClassFixture<TFixture>\' or \'Xunit.ICollectionFixture<TFixture>\' should add a constructor argument of type TFixture")]
public abstract class BaseEntityControllerTests : IClassFixture<WebAppFactory<Program>>
{
    protected HttpClient? Client { get; init; }

    protected abstract HrimEntity GetCreateRequest();
    protected abstract HrimEntity GetUpdateRequest();

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("352246af-9681")]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552-352246af-9681-4aae-9c2c-6faddcb2e552")]
    public async Task GetById_Given_Wrong_Id_Returns_BadRequest(string url)
    {
        var response = await Client!.GetAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
            .ContainsKey(nameof(ByIdRequest.Id).ToSnakeCase())
            .Should().BeTrue();
    }

    [Fact]
    public async Task Create_Given_Id_Returns_BadRequest()
    {
        var createRequest = GetCreateRequest();
        createRequest.Id = Guid.NewGuid();
        var response = await Client!.PostAsync("", TestUtils.PrepareJson(createRequest));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
            .ContainsKey(nameof(createRequest.Id).ToSnakeCase())
            .Should().BeTrue();
    }

    [Fact]
    public async Task Create_Given_Positive_ConcurrentToken_Returns_BadRequest()
    {
        var createRequest = GetCreateRequest();
        createRequest.ConcurrentToken = 1;
        var response = await Client!.PostAsync("", TestUtils.PrepareJson(createRequest));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
            .ContainsKey(nameof(createRequest.ConcurrentToken).ToSnakeCase())
            .Should().BeTrue();
    }

    [Fact]
    public async Task Update_Given_Empty_Id_Returns_BadRequest()
    {
        var updateRequest = GetUpdateRequest();
        updateRequest.Id = Guid.Empty;
        var response = await Client!.PutAsync("", TestUtils.PrepareJson(updateRequest));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
            .ContainsKey(nameof(updateRequest.Id).ToSnakeCase())
            .Should().BeTrue();
    }

    [Fact]
    public async Task Update_Given_0_ConcurrentToken_Returns_BadRequest()
    {
        var updateRequest = GetCreateRequest();
        updateRequest.ConcurrentToken = 0;
        var response = await Client!.PutAsync("", TestUtils.PrepareJson(updateRequest));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
            .ContainsKey(nameof(updateRequest.ConcurrentToken).ToSnakeCase())
            .Should().BeTrue();
    }
}