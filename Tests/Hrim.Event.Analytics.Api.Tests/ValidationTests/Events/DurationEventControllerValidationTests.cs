using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Api.Tests.Infrastructure.TestingHost;
using Hrim.Event.Analytics.Api.V1.Models;
using Hrimsoft.Core.Extensions;
using Hrimsoft.StringCases;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Hrim.Event.Analytics.Api.Tests.ValidationTests.Events;

[ExcludeFromCodeCoverage]
public class DurationEventControllerValidationTests: BaseEventControllerValidationTests
{
    /// <summary> Correct duration event create request </summary>
    private readonly DurationEventCreateRequest _durationEventCreateRequest = new() {
        CreatedById = Guid.NewGuid(),
        EventTypeId = Guid.NewGuid(),
        StartedAt = new DateTimeOffset(year: 2020,
                                       month: 09,
                                       day: 1,
                                       hour: 15,
                                       minute: 0,
                                       second: 0,
                                       offset: TimeSpan.Zero),
        FinishedAt = new DateTimeOffset(year: 2020,
                                        month: 09,
                                        day: 1,
                                        hour: 16,
                                        minute: 0,
                                        second: 0,
                                        offset: TimeSpan.Zero)
    };

    /// <summary> Correct duration event create request </summary>
    private readonly DurationEventUpdateRequest _durationEventUpdateRequest = new() {
        Id          = Guid.NewGuid(),
        CreatedById = Guid.NewGuid(),
        EventTypeId = Guid.NewGuid(),
        StartedAt = new DateTimeOffset(year: 2020,
                                       month: 09,
                                       day: 1,
                                       hour: 15,
                                       minute: 0,
                                       second: 0,
                                       offset: TimeSpan.Zero),
        FinishedAt = new DateTimeOffset(year: 2020,
                                        month: 09,
                                        day: 1,
                                        hour: 16,
                                        minute: 0,
                                        second: 0,
                                        offset: TimeSpan.Zero),
        CreatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
        ConcurrentToken = 1
    };

    public DurationEventControllerValidationTests(EventAnalyticsWebAppFactory<Program> factory): base(factory: factory) { Client = factory.GetClient(baseUrl: "v1/event/duration/"); }

    /// <summary> Correct create event request  </summary>
    protected override DurationEventCreateRequest GetCreateRequest() {
        return new DurationEventCreateRequest {
            StartedAt  = DateTimeOffset.Now,
            FinishedAt = DateTimeOffset.Now.AddHours(hours: 1)
        };
    }

    /// <summary> Correct update event request  </summary>
    protected override DurationEventUpdateRequest GetUpdateRequest() {
        return new DurationEventUpdateRequest {
            Id              = Guid.NewGuid(),
            ConcurrentToken = 1,
            StartedAt       = DateTimeOffset.Now,
            FinishedAt      = DateTimeOffset.Now.AddHours(hours: 1)
        };
    }

    protected override BaseEvent GetBaseEventCreateRequest() { return _durationEventCreateRequest; }

    protected override BaseEvent GetBaseEventUpdateRequest() { return _durationEventUpdateRequest; }

    [Theory]
    [InlineData("")]
    [InlineData("0001-01-01T00:00:00.0000000+00:00")]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552")]
    public async Task Create_Given_Wrong_StartedAt_Returns_BadRequest(string startedAt) {
        _durationEventCreateRequest.StartedAt = DateTimeOffset.MinValue;
        var payload = JsonConvert.SerializeObject(value: _durationEventCreateRequest, settings: JsonSettings)
                                 .Replace(DateTimeOffset.MinValue.ToString(format: "O"), newValue: startedAt);

        var response = await Client!.PostAsync(requestUri: "", new StringContent(content: payload, encoding: Encoding.UTF8, mediaType: "application/json"));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(DurationEvent.StartedAt).ToSnakeCase())
                      .Should()
                      .BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("0001-01-01T00:00:00.0000000+00:00")]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552")]
    public async Task Update_Given_Wrong_StartedAt_Returns_BadRequest(string startedAt) {
        _durationEventUpdateRequest.StartedAt = DateTimeOffset.MinValue;
        var payload = JsonConvert.SerializeObject(value: _durationEventUpdateRequest, settings: JsonSettings)
                                 .Replace(DateTimeOffset.MinValue.ToString(format: "O"), newValue: startedAt);

        var response = await Client!.PutAsync(requestUri: "", new StringContent(content: payload, encoding: Encoding.UTF8, mediaType: "application/json"));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(DurationEvent.StartedAt).ToSnakeCase())
                      .Should()
                      .BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("0001-01-01T00:00:00.0000000+00:00")]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552")]
    public async Task Create_Given_Wrong_FinishedAt_Returns_BadRequest(string finishedAt) {
        _durationEventCreateRequest.FinishedAt = DateTimeOffset.MinValue;
        var payload = JsonConvert.SerializeObject(value: _durationEventCreateRequest, settings: JsonSettings)
                                 .Replace(DateTimeOffset.MinValue.ToString(format: "O"), newValue: finishedAt);

        var response = await Client!.PostAsync(requestUri: "", new StringContent(content: payload, encoding: Encoding.UTF8, mediaType: "application/json"));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(DurationEvent.FinishedAt).ToSnakeCase())
                      .Should()
                      .BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("0001-01-01T00:00:00.0000000+00:00")]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552")]
    public async Task Update_Given_Wrong_FinishedAt_Returns_BadRequest(string finishedAt) {
        _durationEventUpdateRequest.FinishedAt = DateTimeOffset.MinValue;
        var payload = JsonConvert.SerializeObject(value: _durationEventUpdateRequest, settings: JsonSettings)
                                 .Replace(DateTimeOffset.MinValue.ToString(format: "O"), newValue: finishedAt);

        var response = await Client!.PutAsync(requestUri: "", new StringContent(content: payload, encoding: Encoding.UTF8, mediaType: "application/json"));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(DurationEvent.FinishedAt).ToSnakeCase())
                      .Should()
                      .BeTrue();
    }

    [Theory]
    [InlineData("0001-01-03T00:00:00.0000000+00:00", "0001-01-02T00:00:00.0000000+00:00")]
    [InlineData("0001-01-03T00:00:00.0000000+00:00", "0001-01-03T00:00:00.0000000+00:00")]
    public async Task Create_Given_FinishedAt_BeforeOrSame_StartedAt_Returns_BadRequest(string startedAt,
                                                                                        string finishedAt) {
        _durationEventCreateRequest.FinishedAt = DateTimeOffset.MinValue.AddDays(days: 1).TruncateToSeconds();
        _durationEventCreateRequest.StartedAt  = DateTimeOffset.MinValue;
        var finishedTemplate = _durationEventCreateRequest.FinishedAt.Value.ToString(format: "O");
        var payload = JsonConvert.SerializeObject(value: _durationEventCreateRequest, settings: JsonSettings)
                                 .Replace(DateTimeOffset.MinValue.ToString(format: "O"), newValue: startedAt)
                                 .Replace(oldValue: finishedTemplate,                    newValue: finishedAt);

        var response = await Client!.PostAsync(requestUri: "", new StringContent(content: payload, encoding: Encoding.UTF8, mediaType: "application/json"));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(DurationEvent.StartedAt).ToSnakeCase())
                      .Should()
                      .BeTrue();
    }

    [Theory]
    [InlineData("0001-01-03T00:00:00.0000000+00:00", "0001-01-02T00:00:00.0000000+00:00")]
    [InlineData("0001-01-03T00:00:00.0000000+00:00", "0001-01-03T00:00:00.0000000+00:00")]
    public async Task Update_Given_FinishedAt_BeforeOrSame_StartedAt_Returns_BadRequest(string startedAt,
                                                                                        string finishedAt) {
        _durationEventUpdateRequest.FinishedAt = DateTimeOffset.MinValue.AddDays(days: 1).TruncateToSeconds();
        _durationEventUpdateRequest.StartedAt  = DateTimeOffset.MinValue;
        var finishedTemplate = _durationEventUpdateRequest.FinishedAt.Value.ToString(format: "O");
        var payload = JsonConvert.SerializeObject(value: _durationEventUpdateRequest, settings: JsonSettings)
                                 .Replace(DateTimeOffset.MinValue.ToString(format: "O"), newValue: startedAt)
                                 .Replace(oldValue: finishedTemplate,                    newValue: finishedAt);

        var response = await Client!.PutAsync(requestUri: "", new StringContent(content: payload, encoding: Encoding.UTF8, mediaType: "application/json"));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(DurationEvent.StartedAt).ToSnakeCase())
                      .Should()
                      .BeTrue();
    }
}