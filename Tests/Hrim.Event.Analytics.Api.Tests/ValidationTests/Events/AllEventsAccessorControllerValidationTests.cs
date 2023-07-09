using System.Net;
using FluentAssertions;
using Hrim.Event.Analytics.Api.Tests.Infrastructure.TestingHost;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Hrim.Event.Analytics.Api.Tests.ValidationTests.Events;

public class AllEventsAccessorControllerValidationTests: IClassFixture<EventAnalyticsWebAppFactory<Program>>
{
    private readonly HttpClient? _client;

    public AllEventsAccessorControllerValidationTests(EventAnalyticsWebAppFactory<Program> factory) { _client = factory.GetClient(baseUrl: "v1/event/"); }

    [Theory]
    [InlineData("?start=&end=2022-09-30")]
    [InlineData("?start=&end=")]
    [InlineData("?start=hello&end=world")]
    public async Task Given_Wrong_Start_Dates_Returns_BadRequest(string url) {
        var response = await _client!.GetAsync(requestUri: url);
        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors.ContainsKey(key: "start").Should().BeTrue();
    }

    [Theory]
    [InlineData("?start=2022-09-01&end=")]
    [InlineData("?start=&end=")]
    [InlineData("?start=hello&end=world")]
    [InlineData("?start=2022-09-30&end=2022-09-01")]
    public async Task Given_Wrong_End_Dates_Returns_BadRequest(string url) {
        var response = await _client!.GetAsync(requestUri: url);
        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors.ContainsKey(key: "end").Should().BeTrue();
    }
}