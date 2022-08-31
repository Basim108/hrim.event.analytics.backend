using System.Net;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;
using Hrim.Event.Analytics.Api.V1.Models;
using Hrimsoft.StringCases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

namespace Hrim.Event.Analytics.Api.Tests.Controllers;

/// <summary>
/// Tests entity properties and common endpoints validation
/// </summary>
public abstract class BaseEntityControllerTests<TEntity>: IClassFixture<WebAppFactory<Program>>
    where TEntity : HrimEntity {
    protected readonly HttpClient Client;

    /// <summary>
    /// Creates and setup http client 
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="baseUrl">must be ended with forward slash</param>
    protected BaseEntityControllerTests(WebAppFactory<Program> factory, string baseUrl) {
        Client = factory.CreateClient(new WebApplicationFactoryClientOptions {
            BaseAddress = new Uri(baseUrl)
        });
    }

    protected abstract TEntity GetCreateRequestEntity();

    protected abstract TEntity GetUpdateRequestEntity();
    
    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("352246af-9681")]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552-352246af-9681-4aae-9c2c-6faddcb2e552")]
    public async Task GetById_Given_Wrong_Id_Returns_BadRequest(string url) {
        var response = await Client.GetAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(ByIdRequest.Id).ToSnakeCase())
                      .Should().BeTrue();
    }

    [Fact]
    public async Task Create_Given_Id_Returns_BadRequest() {
        var createRequest = GetCreateRequestEntity();
        createRequest.Id = Guid.NewGuid();
        var response = await Client.PostAsync("", TestUtils.PrepareJson(createRequest));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(createRequest.Id).ToSnakeCase())
                      .Should().BeTrue();
    }

    [Fact]
    public async Task Create_Given_Positive_ConcurrentToken_Returns_BadRequest() {
        var createRequest = GetCreateRequestEntity();
        createRequest.ConcurrentToken = 1;
        var response = await Client.PostAsync("", TestUtils.PrepareJson(createRequest));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(createRequest.ConcurrentToken).ToSnakeCase())
                      .Should().BeTrue();
    }
    
    [Fact]
    public async Task Update_Given_Empty_Id_Returns_BadRequest() {
        var updateRequest = GetUpdateRequestEntity();
        updateRequest.Id = Guid.Empty;
        var response = await Client.PutAsync("", TestUtils.PrepareJson(updateRequest));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(updateRequest.Id).ToSnakeCase())
                      .Should().BeTrue();
    }

    [Fact]
    public async Task Update_Given_0_ConcurrentToken_Returns_BadRequest() {
        var updateRequest = GetCreateRequestEntity();
        updateRequest.ConcurrentToken = 0;
        var response = await Client.PutAsync("", TestUtils.PrepareJson(updateRequest));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(updateRequest.ConcurrentToken).ToSnakeCase())
                      .Should().BeTrue();
    }
}